using System.Threading;
using System.Threading.Tasks;
namespace TradingSystem.DataCollector.Services;

public interface IDataCollectorService
{
    Task StartAsync(CancellationToken ct = default);
    Task StopAsync();
}