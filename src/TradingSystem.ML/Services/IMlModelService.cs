using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using TradingSystem.ML.Models;

namespace TradingSystem.ML.Services
{
    public interface IMlModelService
    {
        Task<(ITransformer Model, DataViewSchema Schema)> TrainAsync(IEnumerable<MLFeatures> dataset, CancellationToken ct = default);
        Task<Microsoft.ML.Data.BinaryClassificationMetrics> EvaluateAsync(ITransformer model, IEnumerable<MLFeatures> testDataset, CancellationToken ct = default);
        Task<PricePrediction> PredictAsync(MLFeatures sample, CancellationToken ct = default);
        Task SaveModelAsync(ITransformer model, DataViewSchema schema, string path);
        Task<(ITransformer Model, DataViewSchema Schema)> LoadModelAsync(string path);
    }
}
