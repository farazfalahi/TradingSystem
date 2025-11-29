namespace TradingSystem.Api.Dto;

public class PredictionDto
{
    public string Symbol { get; set; } = string.Empty;
    public double ProbabilityUp { get; set; }
    public double ProbabilityDown => 1 - ProbabilityUp;
    public string ModelVersion { get; set; } = "v1";
    public string Note { get; set; } = string.Empty;
}