using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TradingSystem.Infrastructure.Messaging
{
    public class RabbitMqService : IRabbitMqService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqService(string hostName = "localhost")
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                DispatchConsumersAsync = true 
            };

            _connection = factory.CreateConnection();   
            _channel = _connection.CreateModel();      
        }

        public Task PublishAsync<T>(string queueName, T message)
        {
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var props = _channel.CreateBasicProperties();
            props.Persistent = true; 

            _channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: props,
                body: body);

            return Task.CompletedTask;
        }

        public Task SubscribeAsync<T>(string queueName, Func<T, Task> handler)
        {
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<T>(json);

                    if (message != null)
                        await handler(message);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception)
                {
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
            }
            catch { }
        }
    }
}
