using Dapper;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Application.Abstractions.Market;
using TradingSystem.Application.DTOs;
namespace TradingSystem.Infrastructure.Market;

public class SymbolRepository : ISymbolRepository
{
    private readonly IDbConnection _db;

    public SymbolRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<SymbolDto> GetSymbolAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var sql = @"
                  SELECT TOP 1
                      Symbol,
                      Name,
                      LastPrice,
                      UpdatedAt
                  FROM Market.Symbols
                  WHERE Symbol = @symbol";

        return await _db.QueryFirstOrDefaultAsync<SymbolDto>(sql, new { symbol });
    }
}