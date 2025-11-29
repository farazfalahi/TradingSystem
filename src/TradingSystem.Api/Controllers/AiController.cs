using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Application.Services;
namespace TradingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly MarketQueryService _marketQueryService;
    public AiController(MarketQueryService marketQueryService) => _marketQueryService = marketQueryService;

    /// <summary>
    /// این کنترلر مربوط به مسیرهای مرتبط با هوش مصنوعی و یادگیری ماشین مانند پیش‌بینی قیمت است.
    /// این کنترلر یک نماد را دریافت می‌کند و پیش‌بینی کوتاه‌مدت مبتنی بر یادگیری ماشی  بازمی‌گرداند.
    /// </summary>
    [HttpGet("prediction/{symbol}")]
    public async Task<IActionResult> GetPrediction(string symbol, CancellationToken cancellationToken)
    {
        var prediction = await _marketQueryService.GetPredictionAsync(symbol, cancellationToken);
        return Ok(prediction);
    }
}