using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Application.DTOs;
namespace TradingSystem.Application.Abstractions.Market;

public interface ISymbolRepository
{
    Task<SymbolDto> GetSymbolAsync(string symbol, CancellationToken cancellationToken = default);
}