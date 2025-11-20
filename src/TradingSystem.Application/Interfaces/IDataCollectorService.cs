using System.Threading.Tasks;

namespace TradingSystem.Application.Interfaces;

public interface IDataCollectorService
{
    Task StartCollectingAsync();
    Task StopCollectingAsync();
}