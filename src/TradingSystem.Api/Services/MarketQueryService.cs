using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Api.Dto;
using TradingSystem.Application.Services; // or TradingSystem.Application.Interfaces
using TradingSystem.Application.Dto; // if MarketTickDto exists
using TradingSystem.Infrastructure.Messaging;

namespace TradingSystem.Api.Services
{
    public class MarketQueryService : IMarketQueryService
    {
        private readonly TradingSystem.Application.Services.IMarketDataService _marketDataService;
        private readonly TradingSystem.Infrastructure.Messaging.IRabbitMqService _mq;
        private readonly TradingSystem.Api.Clients.IMarketCollectorClient? _collectorClient;

        // constructor supports optional Refit client for direct collector calls
        public MarketQueryService(
            TradingSystem.Application.Services.IMarketDataService marketDataService,
            TradingSystem.Infrastructure.Messaging.IRabbitMqService mq,
            TradingSystem.Api.Clients.IMarketCollectorClient? collectorClient = null)
        {
            _marketDataService = marketDataService;
            _mq = mq;
            _collectorClient = collectorClient;
        }

        public async Task<IEnumerable<InstrumentDto>> GetInstrumentsAsync(CancellationToken ct = default)
        {
            var instruments = await _marketDataService.GetAllInstrumentsAsync();
            return instruments.Select(i => new InstrumentDto
            {
                InstrumentId = i.Id,
                Symbol = i.Symbol,
                Name = i.Name,
                Exchange = i.Exchange,
                Currency = i.Currency
            });
        }

        public async Task<CandleDto?> GetLatestAsync(string symbol, CancellationToken ct = default)
        {
            var inst = await _marketDataService.GetInstrumentBySymbolAsync(symbol);
            if (inst == null) return null;

            var latest = await _marketDataService.GetLatestAsync(inst.Id, TradingSystem.Domain.Enums.AggregationInterval.OneMinute);
            if (latest == null) return null;

            return new CandleDto
            {
                InstrumentId = latest.InstrumentId,
                Symbol = symbol,
                Timestamp = latest.Timestamp,
                Open = latest.Open,
                High = latest.High,
                Low = latest.Low,
                Close = latest.Close,
                Volume = latest.Volume,
                Interval = latest.Interval.ToString()
            };
        }

        public async Task<IEnumerable<CandleDto>> GetHistoryAsync(string symbol, DateTime from, DateTime to, string interval, CancellationToken ct = default)
        {
            var inst = await _marketDataService.GetInstrumentBySymbolAsync(symbol);
            if (inst == null) return Enumerable.Empty<CandleDto>();

            if (!Enum.TryParse<TradingSystem.Domain.Enums.AggregationInterval>(interval, true, out var agg))
            {
                agg = TradingSystem.Domain.Enums.AggregationInterval.OneMinute;
            }

            var data = await _marketDataService.GetHistoryAsync(inst.Id, from, to, agg);
            return data.Select(d => new CandleDto
            {
                InstrumentId = d.InstrumentId,
                Symbol = symbol,
                Timestamp = d.Timestamp,
                Open = d.Open,
                High = d.High,
                Low = d.Low,
                Close = d.Close,
                Volume = d.Volume,
                Interval = d.Interval.ToString()
            });
        }

        public async Task<PredictionDto> GetPredictionAsync(string symbol, CancellationToken ct = default)
        {
            // اگر ML در Worker محاسبه شده باشد، Query از DB لازم است.
            // اینجا نمونه ساده‌ای برمی‌گردانیم (یا از collectorClient استفاده کن)
            // TODO: بجای logic زیر، از پیکربندی ML repo استفاده کن.

            // نمونه stub
            await Task.CompletedTask;
            return new PredictionDto
            {
                Symbol = symbol,
                ProbabilityUp = 0.52,
                ModelVersion = "v1",
                Note = "Sample prediction — replace with real model query"
            };
        }

        public async Task TriggerCollectAsync(TriggerRequest req, CancellationToken ct = default)
        {
            // 1) سعی کن از Refit client استفاده کنی اگر پیکربندی شده
            if (_collectorClient is not null)
            {
                try
                {
                    await _collectorClient.TriggerCollectAsync(req);
                    return;
                }
                catch
                {
                    // fallback to RabbitMQ publish
                }
            }

            // 2) fallback: publish a message to RabbitMQ
            await _mq.PublishAsync("collector.trigger", req);
        }
    }
}
