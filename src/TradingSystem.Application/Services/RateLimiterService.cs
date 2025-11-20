using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TradingSystem.Application.Interfaces;
using TradingSystem.Domain.Enums;

namespace TradingSystem.Application.Services;

public class RateLimiterService : IRateLimiterService
{
    private readonly ConcurrentDictionary<DataSourceType, (DateTime lastReset, int count)> _counters = new();

    private readonly int _defaultRequestsPerMinute = 500;

    public Task<bool> CanRequestAsync(DataSourceType sourceType)
    {
        var now = DateTime.UtcNow;
        if (!_counters.TryGetValue(sourceType, out var value) || (now - value.lastReset).TotalMinutes >= 1)
        {
            _counters[sourceType] = (now, 0);
            return Task.FromResult(true);
        }

        return Task.FromResult(value.count < _defaultRequestsPerMinute);
    }

    public Task RegisterRequestAsync(DataSourceType sourceType)
    {
        var now = DateTime.UtcNow;
        _counters.AddOrUpdate(sourceType,
            (_) => (now, 1),
            (_, old) =>
            {
                if ((now - old.lastReset).TotalMinutes >= 1)
                    return (now, 1);
                return (old.lastReset, old.count + 1);
            });
        return Task.CompletedTask;
    }

    public Task WaitForSlotAsync()
    {
        throw new NotImplementedException();
    }
}