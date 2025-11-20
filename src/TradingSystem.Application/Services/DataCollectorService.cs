using System;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Application.Interfaces;

namespace TradingSystem.Application.Services;

public class DataCollectorService : IDataCollectorService
{
    private readonly IMarketDataService _marketDataService;
    private readonly IRateLimiterService _rateLimiterService;
    private CancellationTokenSource _cts;

    public DataCollectorService(IMarketDataService marketDataService, IRateLimiterService rateLimiterService)
    {
        _marketDataService = marketDataService;
        _rateLimiterService = rateLimiterService;
    }

    public Task StartCollectingAsync()
    {
        _cts = new CancellationTokenSource();
        Task.Run(() => CollectLoopAsync(_cts.Token));
        return Task.CompletedTask;
    }

    public Task StopCollectingAsync()
    {
        _cts?.Cancel();
        return Task.CompletedTask;
    }

    private async Task CollectLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // TODO: اضافه کردن logic واقعی برای خواندن داده‌ها
            await Task.Delay(1000, token);
        }
    }
}