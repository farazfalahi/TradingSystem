using System;
namespace TradingSystem.DataCollector.Utils;

public class BackoffPolicy
{
    private readonly TimeSpan _maxDelay = TimeSpan.FromSeconds(60);
    private int _attempt = 0;

    public TimeSpan NextDelay()
    {
        _attempt++;
        var delay = TimeSpan.FromSeconds(Math.Min(_maxDelay.TotalSeconds, Math.Pow(2, _attempt)));
        return delay;
    }

    public void Reset() => _attempt = 0;
}