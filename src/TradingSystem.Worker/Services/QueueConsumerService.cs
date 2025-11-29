using System.Threading;
using System.Threading.Tasks;
namespace TradingSystem.Worker.Services;

//public class QueueConsumerService : IQueueConsumerService
//{
//    private readonly IMarketDataProcessor _processor;
//    private readonly IRabbitMqConsumer _consumer;

//    public QueueConsumerService(IMarketDataProcessor processor, IRabbitMqConsumer consumer)
//    {
//        _processor = processor;
//        _consumer = consumer;
//    }

//    public async Task StartAsync(CancellationToken ct)
//    {
//        await foreach (var msg in _consumer.ReadMessagesAsync(ct))
//        {
//            await _processor.ProcessAsync(msg, ct);
//        }
//    }
//}
