using System;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
namespace TradingSystem.DataCollector.Utils;

public class AdaptiveRateLimiter
{
    private TokenBucketRateLimiter _limiter;
    private readonly object _lock = new();

    public int CurrentRpm { get; private set; }
    public int MinRpm { get; init; } = 60;

    public AdaptiveRateLimiter(int initialRpm, int minRpm = 60)
    {
        CurrentRpm = initialRpm;
        MinRpm = minRpm;
        RebuildLimiter();
    }

    private void RebuildLimiter()
    {
        var permitsPerSecond = Math.Max(1, CurrentRpm / 60);
        var options = new TokenBucketRateLimiterOptions
        {
            TokenLimit = permitsPerSecond,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 1000,
            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
            TokensPerPeriod = permitsPerSecond,
            AutoReplenishment = true
        };
        _limiter?.Dispose();
        _limiter = new TokenBucketRateLimiter(options);
    }

    public async Task<bool> AcquireAsync(CancellationToken ct = default)
    {
        var lease = await _limiter.AcquireAsync(1, ct);
        return lease.IsAcquired;
    }

    public void DecreaseRpm(int percent = 50)
    {
        lock (_lock)
        {
            var newRpm = Math.Max(MinRpm, CurrentRpm * (100 - percent) / 100);
            if (newRpm != CurrentRpm)
            {
                CurrentRpm = newRpm;
                RebuildLimiter();
            }
        }
    }

    public void IncreaseRpm(int percent = 10, int maxRpm = 600)
    {
        lock (_lock)
        {
            var newRpm = Math.Min(maxRpm, CurrentRpm * (100 + percent) / 100);
            if (newRpm != CurrentRpm)
            {
                CurrentRpm = newRpm;
                RebuildLimiter();
            }
        }
    }
}
