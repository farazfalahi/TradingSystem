using System;
using System.Threading.Tasks;
using TradingSystem.Application.DTOs;

namespace TradingSystem.Infrastructure.Messaging;

public class RabbitMqPublisher : IWebhookPublisher
{
    private readonly IRabbitMqService _mq;
    private readonly string _queueName;

    public RabbitMqPublisher(IRabbitMqService mq, string queueName = "market-ticks")
    {
        _mq = mq ?? throw new ArgumentNullException(nameof(mq));
        _queueName = queueName;
    }

    public Task PublishTickAsync(MarketTickDto tick)
    {
        // Fire-and-forget wrapping - caller can await if wants
        return _mq.PublishAsync(_queueName, tick);
    }
}