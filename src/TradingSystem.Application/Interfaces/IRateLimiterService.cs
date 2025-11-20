using System.Threading.Tasks;
using TradingSystem.Domain.Enums;

namespace TradingSystem.Application.Interfaces;

public interface IRateLimiterService
{
    Task<bool> CanRequestAsync(DataSourceType sourceType);
    Task RegisterRequestAsync(DataSourceType sourceType);
    Task WaitForSlotAsync();
}