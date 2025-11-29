//using FluentAssertions;
//using TradingSystem.Infrastructure.Messaging;
//using Xunit;

//namespace TradingSystem.Tests.Infrastructure;

//public class RabbitMqPublisherTests
//{
//    [Fact]
//    public async Task PublishAsync_Should_Not_Throw()
//    {
//        var publisher = new RabbitMqPublisher("localhost"); // mocked environment

//        var act = async () => await publisher.PublishAsync("test.queue", new { Price = 100 });

//        await act.Should().NotThrowAsync();
//    }
//}
