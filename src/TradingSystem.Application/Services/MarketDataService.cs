using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Application.DTOs;
using TradingSystem.Application.Interfaces;
using TradingSystem.Domain.Entities;
using TradingSystem.Domain.Enums;

namespace TradingSystem.Application.Services;

public class MarketDataService : IMarketDataService
{
    private readonly List<MarketData> _store = new();

    public Task<MarketData> GetLatestAsync(Guid instrumentId, AggregationInterval interval)
    {
        var latest = _store.FindLast(md => md.InstrumentId == instrumentId && md.Interval == interval);
        return Task.FromResult(latest);
    }

    public Task<IEnumerable<MarketData>> GetHistoryAsync(Guid instrumentId, DateTime from, DateTime to, AggregationInterval interval)
    {
        IEnumerable<MarketData> history = _store.FindAll(md => md.InstrumentId == instrumentId && md.Interval == interval && md.Timestamp >= from && md.Timestamp <= to);
        return Task.FromResult(history);
    }

    public Task SaveMarketDataAsync(MarketData data)
    {
        _store.Add(data);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<MarketData>> GetAllInstrumentsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MarketData>> GetHistoricalDataAsync(object symbol)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<MarketData> SubscribeRealtimeAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    public Task SaveMarketDataBatchAsync(IEnumerable<MarketData> batch)
    {
        throw new NotImplementedException();
    }

    Task<IEnumerable<Instrument>> IMarketDataService.GetAllInstrumentsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Instrument> GetInstrumentBySymbolAsync(string symbol)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MarketData>> GetHistoricalDataBySymbolAsync(string symbol, DateTime from, DateTime to, AggregationInterval interval)
    {
        throw new NotImplementedException();
    }

    IAsyncEnumerable<MarketTickDto> IMarketDataService.SubscribeRealtimeAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}