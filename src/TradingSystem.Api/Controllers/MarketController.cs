using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TradingSystem.Api.Services;
namespace TradingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarketController : ControllerBase
{
    private readonly IMarketQueryService _svc;

    public MarketController(IMarketQueryService svc)
    {
        _svc = svc;
    }

    [HttpGet("{symbol}/latest")]
    public async Task<IActionResult> GetLatest(string symbol)
    {
        var dto = await _svc.GetLatestAsync(symbol);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpGet("{symbol}/history")]
    public async Task<IActionResult> GetHistory(string symbol, [FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string interval = "OneMinute")
    {
        var list = await _svc.GetHistoryAsync(symbol, from, to, interval);
        return Ok(list);
    }
}