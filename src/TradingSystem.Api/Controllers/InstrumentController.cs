using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TradingSystem.Api.Services;

namespace TradingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstrumentController : ControllerBase
    {
        private readonly IMarketQueryService _svc;

        public InstrumentController(IMarketQueryService svc)
        {
            _svc = svc;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _svc.GetInstrumentsAsync();
            return Ok(list);
        }
    }
}
