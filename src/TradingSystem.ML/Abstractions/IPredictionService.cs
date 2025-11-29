using System.Threading;
using System.Threading.Tasks;
using TradingSystem.ML.Models;

namespace TradingSystem.ML.Abstractions;

public interface IPredictionService
{
    Task<PredictionResult> PredictAsync(string symbol, CancellationToken ct = default);
}