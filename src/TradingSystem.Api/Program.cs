using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using System;
using TradingSystem.Api.Clients;
using TradingSystem.Api.Services;
using TradingSystem.Application.Services;
using TradingSystem.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Add controllers & swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health checks
builder.Services.AddHealthChecks();

// Refit client (optional) - DataCollector address from config
var collectorBase = builder.Configuration["Services:Collector"];
if (!string.IsNullOrEmpty(collectorBase))
{
    builder.Services.AddRefitClient<IMarketCollectorClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(collectorBase));
}

// Register application services
// NOTE: replace these with your actual registrations
builder.Services.AddScoped<IMarketQueryService, MarketQueryService>();

// Register IMarketDataService from Application layer (assumed implemented)
builder.Services.AddScoped<TradingSystem.Application.Services.IMarketDataService, TradingSystem.Application.Services.MarketDataService>();

// Register IRabbitMqService (from Infrastructure) — ensure Infrastructure DI is called before or register here
// Example quick registration with defaults (replace with proper DI from Infrastructure project)
builder.Services.AddSingleton<IRabbitMqService>(sp => new RabbitMqService(builder.Configuration["RabbitMq:HostName"] ?? "localhost"));

// Refit client already registered above (optional)
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/healthz");

app.Run();
