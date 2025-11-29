using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Trainers.FastTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.ML.Models;
using TradingSystem.ML.Utils;

namespace TradingSystem.ML.Services
{
    public class MlModelService : IMlModelService
    {
        private readonly MLContext _ml;
        private ITransformer? _model;
        private DataViewSchema? _modelSchema;
        private PredictionEngine<MLFeatures, PricePrediction>? _predictionEngine;

        public MlModelService(int? seed = 42)
        {
            _ml = new MLContext(seed ?? 0);
        }

        public async Task<(ITransformer Model, DataViewSchema Schema)> TrainAsync(IEnumerable<MLFeatures> dataset, CancellationToken ct = default)
        {
            if (dataset == null) throw new ArgumentNullException(nameof(dataset));

            // تبدیل به IDataView
            var data = _ml.Data.LoadFromEnumerable(dataset);

            // time-based split بهتر است ولی اینجا random split می‌کنیم (caller می‌تواند split را خود انجام دهد)
            var split = _ml.Data.TrainTestSplit(data, testFraction: 0.2);

            var featureCols = new[] { nameof(MLFeatures.Close), nameof(MLFeatures.Volume),
                nameof(MLFeatures.SMA5), nameof(MLFeatures.SMA20), nameof(MLFeatures.RSI14), nameof(MLFeatures.Momentum) };

            var pipeline = _ml.Transforms.Concatenate("Features", featureCols)
                .Append(_ml.Transforms.NormalizeMeanVariance("Features"))
                .Append(_ml.BinaryClassification.Trainers.FastTree(new FastTreeBinaryTrainer.Options
                {
                    LabelColumnName = "Label",
                    FeatureColumnName = "Features",
                    NumberOfLeaves = 50,
                    NumberOfTrees = 200,
                    MinimumExampleCountPerLeaf = 20
                }));

            var model = pipeline.Fit(split.TrainSet);

            var preds = model.Transform(split.TestSet);
            var metrics = _ml.BinaryClassification.Evaluate(preds, labelColumnName: "Label");

            // نگه‌داشتن مدل در حافظه
            _model = model;
            _modelSchema = split.TrainSet.Schema;
            _predictionEngine = _ml.Model.CreatePredictionEngine<MLFeatures, PricePrediction>(_model);

            // برگرداندن نتیجه
            return await Task.FromResult((model, split.TrainSet.Schema));
        }

        public async Task<BinaryClassificationMetrics> EvaluateAsync(ITransformer model, IEnumerable<MLFeatures> testDataset, CancellationToken ct = default)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var data = _ml.Data.LoadFromEnumerable(testDataset);
            var preds = model.Transform(data);
            var metrics = _ml.BinaryClassification.Evaluate(preds, labelColumnName: "Label");
            return await Task.FromResult(metrics);
        }

        public async Task<PricePrediction> PredictAsync(MLFeatures sample, CancellationToken ct = default)
        {
            if (_predictionEngine == null)
            {
                if (_model == null || _modelSchema == null)
                {
                    // تلاش برای بارگذاری مدل پیش‌فرض از دیسک
                    var path = MlModelFilePaths.GetModelPath();
                    if (File.Exists(path))
                    {
                        var (m, schema) = await LoadModelAsync(path);
                        _model = m;
                        _modelSchema = schema;
                        _predictionEngine = _ml.Model.CreatePredictionEngine<MLFeatures, PricePrediction>(_model);
                    }
                    else
                    {
                        throw new InvalidOperationException("Model not trained or found on disk. Train or load a model before prediction.");
                    }
                }
                else
                {
                    _predictionEngine = _ml.Model.CreatePredictionEngine<MLFeatures, PricePrediction>(_model);
                }
            }

            var pred = _predictionEngine.Predict(sample);

            return await Task.FromResult(pred);
        }

        public async Task SaveModelAsync(ITransformer model, DataViewSchema schema, string path)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(path)) path = MlModelFilePaths.GetModelPath();

            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            using var fs = File.Create(path);
            _ml.Model.Save(model, schema, fs);

            await Task.CompletedTask;
        }

        public async Task<(ITransformer Model, DataViewSchema Schema)> LoadModelAsync(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("Model file not found", path);
            using var fs = File.OpenRead(path);
            var model = _ml.Model.Load(fs, out var schema);
            _model = model;
            _modelSchema = schema;
            _predictionEngine = _ml.Model.CreatePredictionEngine<MLFeatures, PricePrediction>(_model);
            return await Task.FromResult((model, schema));
        }
    }
}
