using Microsoft.ML;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.ML.Abstractions;
using TradingSystem.ML.Models;

namespace TradingSystem.ML.Services;

public class PredictionService : IPredictionService
{
    private readonly MLContext _ml = new();
    private readonly ITransformer? _model;
    private const string ModelPath = "Models/price-model.zip";

    public PredictionService()
    {
        if (File.Exists(ModelPath))
            _model = _ml.Model.Load(ModelPath, out _);
    }

    public Task<PredictionResult> PredictAsync(string symbol, CancellationToken ct = default)
    {
        // اگر مدل نبود — خروجی Mock
        if (_model == null)
        {
            return Task.FromResult(new PredictionResult
            {
                Symbol = symbol,
                ProbabilityUp = 0.51f,
                ModelVersion = "mock-v0"
            });
        }

        // ورودی نمونه — بعداً از DB جداگانه می‌آید
        var sample = new PriceInput
        {
            Symbol = symbol,
            LastPrice = 100,
            Volume = 12000,
            RSI = 55,
            SMA20 = 102,
            SMA50 = 98
        };

        var engine = _ml.Model.CreatePredictionEngine<PriceInput, PriceOutput>(_model);
        var result = engine.Predict(sample);

        return Task.FromResult(new PredictionResult
        {
            Symbol = symbol,
            ProbabilityUp = result.ProbabilityUp,
            ModelVersion = "v1"
        });
    }
}
