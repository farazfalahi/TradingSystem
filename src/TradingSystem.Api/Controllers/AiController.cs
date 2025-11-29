using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TradingSystem.Api.Services;
namespace TradingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IMarketQueryService _svc;
    public AiController(IMarketQueryService svc) => _svc = svc;

    [HttpGet("prediction/{symbol}")]
    public async Task<IActionResult> GetPrediction(string symbol)
    {
        var p = await _svc.GetPredictionAsync(symbol);
        return Ok(p);
    }
}