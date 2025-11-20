using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Domain.Entities;
using TradingSystem.Infrastructure.Persistence;
namespace TradingSystem.Infrastructure.Repositories;

public class InstrumentRepository : IInstrumentRepository
{
    private readonly TradingSystemDbContext _db;

    public InstrumentRepository(TradingSystemDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Instrument>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Instruments.ToListAsync(ct);
    }

    public async Task<Instrument?> GetBySymbolAsync(string symbol, CancellationToken ct = default)
    {
        return await _db.Instruments.FirstOrDefaultAsync(x => x.Symbol == symbol, ct);
    }

    public async Task AddAsync(Instrument instrument, CancellationToken ct = default)
    {
        await _db.Instruments.AddAsync(instrument, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SaveHistoricalCandlesAsync(string symbol, IEnumerable<MarketData> candles, CancellationToken ct = default)
    {
        var instrument = await GetBySymbolAsync(symbol, ct);
        if (instrument == null)
            throw new InvalidOperationException($"Instrument {symbol} not found.");

        foreach (var c in candles)
        {
            c.InstrumentId = instrument.Id;
            await _db.MarketData.AddAsync(c, ct);
        }

        await _db.SaveChangesAsync(ct);
    }
}
