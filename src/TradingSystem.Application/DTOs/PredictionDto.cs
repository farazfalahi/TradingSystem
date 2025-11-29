namespace TradingSystem.Application.DTOs;

public class PredictionDto
{
    public string Symbol { get; set; }
    public double ProbabilityUp { get; set; }
    public string ModelVersion { get; set; }
    public string Note { get; set; }
}