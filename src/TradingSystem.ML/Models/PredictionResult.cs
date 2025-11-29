namespace TradingSystem.ML.Models;

public class PredictionResult
{
    public string Symbol { get; set; }
    public float ProbabilityUp { get; set; }
    public string ModelVersion { get; set; }
}