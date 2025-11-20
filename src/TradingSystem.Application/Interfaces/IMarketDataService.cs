using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Application.DTOs;
using TradingSystem.Domain.Entities;
using TradingSystem.Domain.Enums;

namespace TradingSystem.Application.Interfaces;

public interface IMarketDataService
{
    // Historical
    Task<IEnumerable<MarketData>> GetHistoryAsync(Guid instrumentId,DateTime from,DateTime to,AggregationInterval interval);

    Task<MarketData> GetLatestAsync(Guid instrumentId,AggregationInterval interval);

    // Saving
    Task SaveMarketDataAsync(MarketData data);
    Task SaveMarketDataBatchAsync(IEnumerable<MarketData> batch);

    // Instruments
    Task<IEnumerable<Instrument>> GetAllInstrumentsAsync();
    Task<Instrument> GetInstrumentBySymbolAsync(string symbol);

    // Convenience
    Task<IEnumerable<MarketData>> GetHistoricalDataBySymbolAsync(string symbol,DateTime from,DateTime to,AggregationInterval interval);

    // Realtime stream (ticks)
    IAsyncEnumerable<MarketTickDto> SubscribeRealtimeAsync(CancellationToken cancellationToken);
}