namespace TradingSystem.Application.DTOs;

public class TriggerRequest
{
    public string Symbol { get; set; } = string.Empty;
    public string From { get; set; }
    public string To { get; set; }
    public string Interval { get; set; } = "1m";
}
