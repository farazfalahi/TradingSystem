using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Application.Interfaces;
using TradingSystem.Domain.Enums;
using TradingSystem.Infrastructure.Messaging;
namespace TradingSystem.DataCollector.HostedService;

public class DataCollectorHostedService : BackgroundService
{
    private readonly ILogger<DataCollectorHostedService> _logger;
    private readonly IMarketDataService _marketDataService;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IRateLimiterService _rateLimiter;

    public DataCollectorHostedService(
        ILogger<DataCollectorHostedService> logger,
        IMarketDataService marketDataService,
        IRabbitMqService rabbitMqService,
        IRateLimiterService rateLimiter)
    {
        _logger = logger;
        _marketDataService = marketDataService;
        _rabbitMqService = rabbitMqService;
        _rateLimiter = rateLimiter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("📡 DataCollector started...");

        // مرحله ۱: دریافت داده‌های تاریخی
        await CollectHistoricalAsync(stoppingToken);

        // مرحله ۲: شروع WebSocket یا Polling برای داده‌های لحظه‌ای
        await CollectRealtimeAsync(stoppingToken);
    }

    private async Task CollectHistoricalAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Downloading historical data...");

        var instruments = await _marketDataService.GetAllInstrumentsAsync();

        // تعریف بازه تاریخی
        var to = DateTime.UtcNow;
        var from = to.AddDays(-90);
        var interval = AggregationInterval.OneMinute;

        foreach (var instrument in instruments)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            await _rateLimiter.WaitForSlotAsync();  // جلوگیری از RateLimit

            // متد جدید با پارامترهای کامل
            var candles = await _marketDataService.GetHistoricalDataBySymbolAsync(
                instrument.Symbol,
                from,
                to,
                interval
            );

            foreach (var candle in candles)
            {
                await _rabbitMqService.PublishAsync("marketdata.historical", candle);
            }

            _logger.LogInformation("📦 Historical sent for: {symbol}", instrument.Symbol);
        }
    }

    private async Task CollectRealtimeAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("📶 Starting real-time listener...");

        await foreach (var tick in _marketDataService.SubscribeRealtimeAsync(stoppingToken))
        {
            await _rateLimiter.WaitForSlotAsync();
            await _rabbitMqService.PublishAsync("marketdata.realtime", tick);
        }
    }
}
