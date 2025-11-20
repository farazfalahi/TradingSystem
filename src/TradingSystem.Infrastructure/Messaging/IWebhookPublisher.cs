using System.Threading.Tasks;
using TradingSystem.Application.DTOs;
namespace TradingSystem.Infrastructure.Messaging;

public interface IWebhookPublisher
{
    Task PublishTickAsync(MarketTickDto tick);
}
