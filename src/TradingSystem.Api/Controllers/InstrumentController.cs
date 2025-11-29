using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TradingSystem.Application.Services;

namespace TradingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstrumentController : ControllerBase
    {
        private readonly MarketQueryService _marketQueryService;

        public InstrumentController(MarketQueryService marketQueryService) => _marketQueryService = marketQueryService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _marketQueryService.GetInstrumentsAsync();
            return Ok(list);
        }
    }
}
