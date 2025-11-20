using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using TradingSystem.DataCollector.Services;
using TradingSystem.DataCollector.Settings;
using TradingSystem.DataCollector.Utils;
using TradingSystem.Infrastructure.Messaging;
namespace TradingSystem.DataCollector;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataCollector(this IServiceCollection services, IConfiguration config)
    {
        // bind settings
        services.Configure<RestSettings>(config.GetSection("MarketApi"));

        var restSettings = new RestSettings();
        config.GetSection("MarketApi").Bind(restSettings);

        // Polly policy: retry with jitter for transient errors (including 429)
        IAsyncPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError() // 5xx, network failure
            .OrResult(msg => (int)msg.StatusCode == 429) // Too Many Requests
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)) + TimeSpan.FromMilliseconds(new Random().Next(0, 1000))
            );

        services.AddHttpClient<IHttpMarketClient, RestMarketClient>(client =>
        {
            client.BaseAddress = new Uri(restSettings.RestBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(restSettings.TimeoutSeconds);
            if (!string.IsNullOrEmpty(restSettings.ApiKey))
            {
                client.DefaultRequestHeaders.Add("X-Api-Key", restSettings.ApiKey);
            }
        })
        .AddPolicyHandler(retryPolicy);

        // Adaptive rate limiter singleton (configurable)
        var maxRpm = int.TryParse(config["RateLimit:MaxRequestsPerMinute"], out var rpm) ? rpm : 600;
        var minRpm = int.TryParse(config["RateLimit:MinRequestsPerMinute"], out var min) ? min : 60;
        services.AddSingleton(new AdaptiveRateLimiter(maxRpm, minRpm));

        // WebSocket client - register factory
        services.AddSingleton<IWebSocketClient>(sp =>
        {
            var wsUrl = config["MarketApi:WebSocketUrl"] ?? restSettings.RestBaseUrl.Replace("http", "ws");
            return new WebSocketMarketClient(wsUrl);
        });

        // Publisher: RabbitMqPublisher which implements IWebhookPublisher
        // Ensure IRabbitMqService is registered (in Infrastructure project's DI registration)
        services.AddSingleton<IWebhookPublisher, RabbitMqPublisher>(sp =>
        {
            var mq = sp.GetRequiredService<TradingSystem.Infrastructure.Messaging.IRabbitMqService>();
            var queue = config["DataCollector:TickQueue"] ?? "market-ticks";
            return new RabbitMqPublisher(mq, queue);
        });

        // Main DataCollector service
        services.AddSingleton<IDataCollectorService, DataCollectorService>();

        // Bootstrapper if needed
        services.AddSingleton<Bootstrapper>();

        return services;
    }
}
