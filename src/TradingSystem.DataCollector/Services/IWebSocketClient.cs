using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Application.DTOs;
namespace TradingSystem.DataCollector.Services;

public interface IWebSocketClient : IAsyncDisposable
{
    Task ConnectAsync(CancellationToken ct = default);
    Task SubscribeAsync(IEnumerable<string> symbols, Func<MarketTickDto, Task> onTick, CancellationToken ct = default);
}