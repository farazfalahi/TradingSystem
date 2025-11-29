using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TradingSystem.Api.Dto;
using TradingSystem.Api.Services;
namespace TradingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectorController : ControllerBase
{
    private readonly IMarketQueryService _svc;
    public CollectorController(IMarketQueryService svc) => _svc = svc;

    [HttpPost("trigger")]
    public async Task<IActionResult> Trigger([FromBody] TriggerRequest req, CancellationToken ct)
    {
        await _svc.TriggerCollectAsync(req, ct);
        return Ok(new { message = "Triggered" });
    }
}