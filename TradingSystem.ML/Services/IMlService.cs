using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ML;
using TradingSystem.ML.Models;
namespace TradingSystem.ML.Services;

public interface IMlService
{
    /// <summary>
    /// Train a model from in-memory data.
    /// Returns the trained ITransformer and the training IDataView schema.
    /// </summary>
    Task<(ITransformer Model, DataViewSchema Schema)> TrainAsync(IEnumerable<TradingData> dataset, CancellationToken ct = default);

    /// <summary>
    /// Evaluate the trained model with test dataset (same schema).
    /// Returns BinaryClassificationMetrics (accuracy, auc, etc).
    /// </summary>
    Task<Microsoft.ML.Data.BinaryClassificationMetrics> EvaluateAsync(ITransformer model, IEnumerable<TradingData> testDataset, CancellationToken ct = default);

    /// <summary>
    /// Predict single instance (returns probability and label).
    /// Load model if not loaded.
    /// </summary>
    Task<TradingPrediction> PredictAsync(TradingData sample, CancellationToken ct = default);

    /// <summary>
    /// Save model to disk.
    /// </summary>
    Task SaveModelAsync(ITransformer model, DataViewSchema inputSchema, string modelPath);

    /// <summary>
    /// Load model from disk.
    /// </summary>
    Task<(ITransformer Model, DataViewSchema Schema)> LoadModelAsync(string modelPath);
}