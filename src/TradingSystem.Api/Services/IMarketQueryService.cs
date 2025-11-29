using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Api.Dto;
namespace TradingSystem.Api.Services;

public interface IMarketQueryService
{
    Task<IEnumerable<InstrumentDto>> GetInstrumentsAsync(CancellationToken ct = default);
    Task<CandleDto?> GetLatestAsync(string symbol, CancellationToken ct = default);
    Task<IEnumerable<CandleDto>> GetHistoryAsync(string symbol, DateTime from, DateTime to, string interval, CancellationToken ct = default);
    Task<PredictionDto> GetPredictionAsync(string symbol, CancellationToken ct = default);
    Task TriggerCollectAsync(TriggerRequest req, CancellationToken ct = default);
}