using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Application.DTOs;
using TradingSystem.DataCollector.DTOs;

namespace TradingSystem.DataCollector.Services;

public interface IHttpMarketClient
{
    Task<IEnumerable<CandleDto>> GetHistoricalCandlesAsync(string symbol, DateTime from, DateTime to, CancellationToken ct = default);
    Task<IEnumerable<MarketTickDto>> GetTicksBatchAsync(IEnumerable<string> symbols, CancellationToken ct = default);
}