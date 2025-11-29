using TradingSystem.ML.Models;

namespace TradingSystem.ML.Services;

public interface IMLPredictor
{
    PredictionOutput Predict(PredictionInput input);
}