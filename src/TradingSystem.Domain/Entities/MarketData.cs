using System;
using TradingSystem.Domain.Enums;
namespace TradingSystem.Domain.Entities;

public class MarketData
{
    public Guid Id { get; private set; }
    public Guid InstrumentId { get; set; }
    public DateTime Timestamp { get; private set; }
    public decimal Open { get; private set; }
    public decimal High { get; private set; }
    public decimal Low { get; private set; }
    public decimal Close { get; private set; }
    public decimal Volume { get; private set; }
    public AggregationInterval Interval { get; private set; }

    public MarketData(Guid instrumentId, DateTime timestamp, decimal open, decimal high, decimal low, decimal close, decimal volume, AggregationInterval interval)
    {
        Id = Guid.NewGuid();
        InstrumentId = instrumentId;
        Timestamp = timestamp;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
        Interval = interval;
    }
}