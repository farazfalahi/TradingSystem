using System;
namespace TradingSystem.Api.Dto;

public class CandleDto
{
    public Guid InstrumentId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }
    public string Interval { get; set; } = "1m";
}
