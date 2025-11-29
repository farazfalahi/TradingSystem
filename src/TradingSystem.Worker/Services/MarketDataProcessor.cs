using System.Threading;
using System.Threading.Tasks;
namespace TradingSystem.Worker.Services;

//public class MarketDataProcessor : IMarketDataProcessor
//{
//    private readonly IIndicatorCalculator _indicators;
//    private readonly IMarketRepository _repo;

//    public MarketDataProcessor(IIndicatorCalculator indicators, IMarketRepository repo)
//    {
//        _indicators = indicators;
//        _repo = repo;
//    }

//    public async Task ProcessAsync(MarketDataMessage message, CancellationToken ct)
//    {
//        await _repo.SaveRawAsync(message.ToEntity());

//        var result = _indicators.Calculate(message);

//        await _repo.SaveIndicatorsAsync(result);
//    }
//}
