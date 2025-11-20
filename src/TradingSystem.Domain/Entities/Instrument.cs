using System;
using TradingSystem.Domain.Enums;

namespace TradingSystem.Domain.Entities;
public class Instrument
{
    public Guid Id { get; private set; }
    public string Symbol { get; private set; }
    public string Name { get; private set; }
    public string Market { get; private set; }
    public string Exchange { get; private set; }
    public string Currency { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Instrument(string symbol, string name, string market, string exchange, string currency)
    {
        if (string.IsNullOrWhiteSpace(symbol)) throw new Exceptions.InvalidInstrumentException("Symbol is required.");
        Id = Guid.NewGuid();
        Symbol = symbol;
        Name = name;
        Market = market;
        Exchange = exchange;
        Currency = currency;
        CreatedAt = DateTime.UtcNow;
    }
}