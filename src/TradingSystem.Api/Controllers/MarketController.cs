using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Application.Services;
using TradingSystem.ML.Services;
namespace TradingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarketController : ControllerBase
{
    private readonly MarketQueryService _marketQueryService;

    public MarketController(MarketQueryService marketQueryService) => _marketQueryService = marketQueryService;

    /// <summary>
    /// اطلاعات یک نماد را بر اساس Symbol برمی‌گرداند.
    /// </summary>
    [HttpGet("symbol/{symbol}")]
    public async Task<IActionResult> GetSymbol(string symbol, CancellationToken ct)
    {
        var result = await _marketQueryService.GetSymbolInfoAsync(symbol, ct);

        if (result is null)
            return NotFound($"نماد '{symbol}' یافت نشد.");

        return Ok(result);
    }
    [HttpGet("{symbol}/latest")]
    public async Task<IActionResult> GetLatest(string symbol, CancellationToken cancellationToken)
    {
        var dto = await _marketQueryService.GetLatestAsync(symbol, cancellationToken);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpGet("{symbol}/history")]
    public async Task<IActionResult> GetHistory(string symbol, [FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string interval , CancellationToken cancellationToken)
    {
        var list = await _marketQueryService.GetHistoryAsync(symbol, from, to, interval,  cancellationToken);
        return Ok(list);
    }
}