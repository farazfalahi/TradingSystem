using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Domain.Entities;
namespace TradingSystem.Infrastructure.Repositories;

public interface IInstrumentRepository
{
    Task<IEnumerable<Instrument>> GetAllAsync(CancellationToken ct = default);
    Task SaveHistoricalCandlesAsync(string symbol, IEnumerable<MarketData> candles, CancellationToken ct = default);
    Task<Instrument?> GetBySymbolAsync(string symbol, CancellationToken ct = default);
    Task AddAsync(Instrument instrument, CancellationToken ct = default);
}