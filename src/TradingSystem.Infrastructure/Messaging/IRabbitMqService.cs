using System;
using System.Threading.Tasks;

namespace TradingSystem.Infrastructure.Messaging;

public interface IRabbitMqService
{
    Task PublishAsync<T>(string queueName, T message);
    Task SubscribeAsync<T>(string queueName, Func<T, Task> handler);
}