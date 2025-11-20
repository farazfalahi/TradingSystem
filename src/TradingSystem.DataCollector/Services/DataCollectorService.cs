using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Application.DTOs;
using TradingSystem.DataCollector.Utils;
using TradingSystem.Infrastructure.Messaging;

namespace TradingSystem.DataCollector.Services;

public class DataCollectorService : IDataCollectorService
{
    private readonly IHttpMarketClient _http;
    private readonly IWebSocketClient _ws;
    private readonly IWebhookPublisher _publisher; // optional: push to RabbitMQ / Worker
    //private readonly IInstrumentService _instrumentService;
    private readonly AdaptiveRateLimiter _rateLimiter;
    private readonly BackoffPolicy _backoff;
    private CancellationTokenSource _cts;

    public DataCollectorService(
        IHttpMarketClient http,
        IWebSocketClient ws,
        AdaptiveRateLimiter rateLimiter,
        IWebhookPublisher publisher)
    {
        _http = http;
        _ws = ws;
        _rateLimiter = rateLimiter;
        _backoff = new BackoffPolicy();
        _publisher = publisher;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        var symbols = new[] { "BTCUSDT", "ETHUSDT", "XRPUSDT" };

        await _ws.ConnectAsync(_cts.Token);

        // await کردن به جای discard تا Exceptions داخل متد آشکار شود
        await _ws.SubscribeAsync(symbols, OnTickAsync, _cts.Token);

        // polling fallback
        _ = PollingLoopAsync(_cts.Token);
    }

    public Task StopAsync()
    {
        _cts?.Cancel();
        return Task.CompletedTask;
    }

    private async Task OnTickAsync(MarketTickDto tick)
    {
        try
        {
            // ذخیره به DB یا ارسال به RabbitMQ
            await _publisher.PublishTickAsync(tick);
            // successful -> possibly increase rate a bit
            _rateLimiter.IncreaseRpm(5);
            _backoff.Reset();
        }
        catch (Exception)
        {
            _rateLimiter.DecreaseRpm(30);
            // log
        }
    }

    private async Task PollingLoopAsync(CancellationToken ct)
    {
        var interval = TimeSpan.FromSeconds(5);
        while (!ct.IsCancellationRequested)
        {
            try
            {
                // نمونه: poll تعداد گروهی از symbols
                var symbols = new List<string> { /* .. groups ..*/ };
                var data = await _http.GetTicksBatchAsync(symbols, ct);
                foreach (var t in data) await _publisher.PublishTickAsync(t);

                // success -> try slightly increasing rpm
                _rateLimiter.IncreaseRpm(2);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == (System.Net.HttpStatusCode)429)
            {
                // 429 -> backoff and reduce rate
                _rateLimiter.DecreaseRpm(50);
                var d = _backoff.NextDelay();
                await Task.Delay(d, ct);
            }
            catch (Exception)
            {
                _rateLimiter.DecreaseRpm(20);
                var d = _backoff.NextDelay();
                await Task.Delay(d, ct);
            }
            await Task.Delay(interval, ct);
        }
    }
}
