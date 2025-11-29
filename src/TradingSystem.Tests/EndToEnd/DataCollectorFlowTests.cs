//using FluentAssertions;
//using Microsoft.EntityFrameworkCore;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System;
//using System.Text;
//using System.Text.Json;
//using TradingSystem.Domain.Entities;
//using TradingSystem.Infrastructure.Messaging;
//using TradingSystem.Infrastructure.Persistence;
//using TradingSystem.Worker;
//using Xunit;

//namespace TradingSystem.Tests.EndToEnd;

//public class DataCollectorFlowTests
//{
//    [Fact]
//    public async Task MarketData_Should_Flow_From_Queue_To_Database()
//    {
//        // 1) Mock DB
//        var options = new DbContextOptionsBuilder<AppDbContext>()
//            .UseInMemoryDatabase("e2e_db")
//            .Options;

//        using var db = new AppDbContext(options);

//        // 2) Create RabbitMQ mock connection (local or testcontainer)
//        var mq = new RabbitMqPublisher("localhost");

//        // 3) Create Worker
//        var worker = new MarketDataWorker(db, mq);

//        // 4) Publish message
//        var sample = new MarketData(
//            Guid.NewGuid(), "BTCUSDT", 100, 95, 105, 98, DateTime.UtcNow
//        );

//        await mq.PublishAsync("marketdata.realtime", sample);

//        // 5) Worker process
//        await worker.ProcessOneAsync("marketdata.realtime");

//        // 6) Assert DB
//        db.MarketData.Count().Should().Be(1);

//        var row = db.MarketData.First();
//        row.Symbol.Should().Be("BTCUSDT");
//    }
//}
