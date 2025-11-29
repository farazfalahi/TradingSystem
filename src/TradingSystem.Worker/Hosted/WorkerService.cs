using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace TradingSystem.Worker.Hosted
{
    public class WorkerService : BackgroundService
    {
        private readonly IQueueConsumerService _consumer;

        public WorkerService(IQueueConsumerService consumer)
        {
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.StartAsync(stoppingToken);
        }
    }
}
