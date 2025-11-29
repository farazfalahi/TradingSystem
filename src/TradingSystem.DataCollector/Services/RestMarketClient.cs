using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Application.DTOs;
using TradingSystem.DataCollector.Services;
namespace TradingSystem.DataCollector.Utils;

public class RestMarketClient : IHttpMarketClient
{
    private readonly HttpClient _http;
    private readonly AdaptiveRateLimiter _rateLimiter;

    public RestMarketClient(HttpClient http, AdaptiveRateLimiter rateLimiter)
    {
        _http = http;
        _rateLimiter = rateLimiter;
    }

    public async Task<IEnumerable<DTOs.CandleDto>> GetHistoricalCandlesAsync(string symbol, DateTime from, DateTime to, CancellationToken ct = default)
    {
        // مثال endpoint: /candles?symbol=XYZ&from=...&to=...
        var url = $"candles?symbol={symbol}&from={from:O}&to={to:O}";
        if (!await _rateLimiter.AcquireAsync(ct)) throw new Exception("Rate limit exceeded");
        var res = await _http.GetFromJsonAsync<IEnumerable<DTOs.CandleDto>>(url, ct);
        return res ?? Array.Empty<DTOs.CandleDto>();
    }

    public async Task<IEnumerable<MarketTickDto>> GetTicksBatchAsync(IEnumerable<string> symbols, CancellationToken ct = default)
    {
        var list = string.Join(",", symbols);
        var url = $"ticks?symbols={list}";
        if (!await _rateLimiter.AcquireAsync(ct)) throw new Exception("Rate limit exceeded");
        var res = await _http.GetFromJsonAsync<IEnumerable<MarketTickDto>>(url, ct);
        return res ?? Array.Empty<MarketTickDto>();
    }
}
