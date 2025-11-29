using FluentAssertions;
using NSubstitute;
using TradingSystem.Application.Interfaces;
using TradingSystem.Application.Services;
using TradingSystem.Domain.Entities;
using Xunit;

namespace TradingSystem.Tests.Application;

//public class MarketDataServiceTests
//{
//    [Fact]
//    public async Task SaveMarketDataAsync_Should_Save_And_Return()
//    {
//        var repo = Substitute.For<IMarketDataRepository>();
//        var service = new MarketDataService(repo);

//        var data = new MarketData(Guid.NewGuid(), "BTCUSDT", 100, 90, 105, 95, DateTime.UtcNow);

//        await service.SaveMarketDataAsync(data);

//        await repo.Received(1).SaveAsync(data);
//    }
//}
