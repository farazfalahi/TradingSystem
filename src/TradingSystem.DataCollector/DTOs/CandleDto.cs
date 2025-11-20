using System;
namespace TradingSystem.DataCollector.DTOs;

public record CandleDto(string Symbol, DateTime StartUtc, decimal Open, decimal High, decimal Low, decimal Close, long Volume);