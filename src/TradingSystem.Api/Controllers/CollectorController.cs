using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Application.DTOs;
using TradingSystem.Application.Services;
namespace TradingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectorController : ControllerBase
{
    private readonly MarketQueryService _marketQueryService;
    public CollectorController(MarketQueryService marketQueryService) => _marketQueryService = marketQueryService;

    [HttpPost("trigger")]
    public async Task<IActionResult> Trigger([FromBody] TriggerRequest triggerRequest, CancellationToken cancellationToken)
    {
        await _marketQueryService.TriggerCollectAsync(triggerRequest, cancellationToken);
        return Ok(new { message = "Triggered" });
    }
}