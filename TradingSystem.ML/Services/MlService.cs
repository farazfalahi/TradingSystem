using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Trainers.FastTree;
using TradingSystem.ML.Models;
using TradingSystem.ML.Utils;
namespace TradingSystem.ML.Services;

public class MlService : IMlService
{
    private readonly MLContext _ml;
    private PredictionEngine<TradingData, TradingPrediction> _predictionEngine;
    private ITransformer? _model;
    private DataViewSchema? _modelSchema;

    public MlService(int? seed = 42)
    {
        _ml = new MLContext(seed ?? 0);
    }

    /// <summary>
    /// Train pipeline:
    /// 1) Concatenate feature columns into "Features"
    /// 2) Normalize (optional)
    /// 3) Trainer: FastTree (binary) with probability output
    /// </summary>
    public async Task<(ITransformer Model, DataViewSchema Schema)> TrainAsync(IEnumerable<TradingData> dataset, CancellationToken ct = default)
    {
        if (dataset == null) throw new ArgumentNullException(nameof(dataset));

        // Convert to IDataView
        var data = _ml.Data.LoadFromEnumerable(dataset);

        // Optionally split into train/test inside this method or caller can do it
        var split = _ml.Data.TrainTestSplit(data, testFraction: 0.2, seed: 123);

        // Features: list all numeric feature columns except Label
        var featureColumns = GetFeatureColumnNames(typeof(TradingData)).Where(c => c != "Label").ToArray();

        // Build pipeline
        var pipeline = _ml.Transforms.Concatenate("Features", featureColumns)
            .Append(_ml.Transforms.NormalizeMeanVariance("Features"))
            // FastTree binary classifier (probability)
            .Append(_ml.BinaryClassification.Trainers.FastTree(new FastTreeBinaryTrainer.Options
            {
                NumberOfLeaves = 50,
                NumberOfTrees = 200,
                MinimumExampleCountPerLeaf = 20,
                FeatureFraction = 1.0,
                LabelColumnName = "Label",
                FeatureColumnName = "Features"
            }));

        // Train
        var model = pipeline.Fit(split.TrainSet);

        // Evaluate on test set
        var predictions = model.Transform(split.TestSet);
        var metrics = _ml.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");

        // store model in memory
        _model = model;
        _modelSchema = split.TrainSet.Schema;
        _predictionEngine = _ml.Model.CreatePredictionEngine<TradingData, TradingPrediction>(_model);

        // return model & schema
        return (model, split.TrainSet.Schema);
    }

    public async Task<BinaryClassificationMetrics> EvaluateAsync(ITransformer model, IEnumerable<TradingData> testDataset, CancellationToken ct = default)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (testDataset == null) throw new ArgumentNullException(nameof(testDataset));

        var data = _ml.Data.LoadFromEnumerable(testDataset);
        var predictions = model.Transform(data);
        var metrics = _ml.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");
        return await Task.FromResult(metrics);
    }

    public async Task<TradingPrediction> PredictAsync(TradingData sample, CancellationToken ct = default)
    {
        if (sample == null) throw new ArgumentNullException(nameof(sample));

        // Ensure model loaded or trained
        if (_predictionEngine == null)
        {
            if (_model == null || _modelSchema == null)
            {
                // try load default model from disk
                var path = MlModelFilePaths.GetModelPath();
                if (File.Exists(path))
                {
                    var (m, schema) = await LoadModelAsync(path);
                    _model = m;
                    _modelSchema = schema;
                    _predictionEngine = _ml.Model.CreatePredictionEngine<TradingData, TradingPrediction>(_model);
                }
                else
                {
                    throw new InvalidOperationException("Model is not trained or loaded. Train or load a model before prediction.");
                }
            }
            else
            {
                _predictionEngine = _ml.Model.CreatePredictionEngine<TradingData, TradingPrediction>(_model);
            }
        }

        var pred = _predictionEngine.Predict(sample);
        return await Task.FromResult(pred);
    }

    public async Task SaveModelAsync(ITransformer model, DataViewSchema inputSchema, string modelPath)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (string.IsNullOrEmpty(modelPath)) modelPath = MlModelFilePaths.GetModelPath();

        var folder = Path.GetDirectoryName(modelPath);
        if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        using (var fs = File.Create(modelPath))
        {
            _ml.Model.Save(model, inputSchema, fs);
        }

        await Task.CompletedTask;
    }

    public async Task<(ITransformer Model, DataViewSchema Schema)> LoadModelAsync(string modelPath)
    {
        if (!File.Exists(modelPath)) throw new FileNotFoundException("Model file not found", modelPath);

        using (var fs = File.OpenRead(modelPath))
        {
            var loaded = _ml.Model.Load(fs, out var schema);
            _model = loaded;
            _modelSchema = schema;
            _predictionEngine = _ml.Model.CreatePredictionEngine<TradingData, TradingPrediction>(_model);
            return (loaded, schema);
        }
    }

    // Utility: reflect properties of TradingData that are feature columns
    private static IEnumerable<string> GetFeatureColumnNames(Type t)
    {
        // Exclude Label field
        var props = t.GetProperties()
            .Where(p => p.Name != "Label" && p.PropertyType == typeof(float))
            .Select(p => p.Name);

        return props;
    }
}