using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TradingSystem.DataCollector;
using TradingSystem.DataCollector.HostedService;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((ctx, cfg) =>
    {
        cfg.AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureServices((ctx, services) =>
    {
        // register infra DI BEFORE data collector registration:
        // e.g., services.AddInfrastructure(ctx.Configuration); // this registers IRabbitMqService etc.

        services.AddDataCollector(ctx.Configuration);

        // Hosted wrapper that starts/stops IDataCollectorService
        services.AddHostedService<DataCollectorHostedService>();
    })
    .Build();

await host.RunAsync();