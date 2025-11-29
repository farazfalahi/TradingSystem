//using FluentAssertions;
//using Microsoft.EntityFrameworkCore;
//using System;
//using TradingSystem.Domain.Entities;
//using TradingSystem.Infrastructure.Persistence;
//using TradingSystem.Infrastructure.Repositories;
//using Xunit;

//namespace TradingSystem.Tests.Infrastructure;

//public class InstrumentRepositoryTests
//{
//    [Fact]
//    public async Task AddAsync_Should_Add_Record_To_Db()
//    {
//        var options = new DbContextOptionsBuilder<AppDbContext>()
//            .UseInMemoryDatabase("test_db")
//            .Options;

//        using var ctx = new AppDbContext(options);
//        var repo = new InstrumentRepository(ctx);

//        var inst = new Instrument("ETHUSDT", "Crypto");

//        await repo.AddAsync(inst);

//        ctx.Instruments.Count().Should().Be(1);
//    }
//}
