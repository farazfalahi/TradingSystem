using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Application.DTOs;
using TradingSystem.DataCollector.DTOs;
namespace TradingSystem.DataCollector.Services;

public class WebSocketMarketClient : IWebSocketClient
{
    private readonly Uri _uri;
    private ClientWebSocket _ws;

    public WebSocketMarketClient(string url)
    {
        _uri = new Uri(url);
    }

    public async Task ConnectAsync(CancellationToken ct = default)
    {
        _ws = new ClientWebSocket();
        await _ws.ConnectAsync(_uri, ct);
    }

    public async Task SubscribeAsync(IEnumerable<string> symbols, Func<MarketTickDto, Task> onTick, CancellationToken ct = default)
    {
        // ارسال پیام subscribe (بستگی به provider)
        var subscribeMsg = JsonSerializer.Serialize(new { action = "subscribe", symbols = symbols });
        var bytes = Encoding.UTF8.GetBytes(subscribeMsg);
        await _ws.SendAsync(bytes, WebSocketMessageType.Text, true, ct);

        var buffer = new byte[8192];
        while (!ct.IsCancellationRequested && _ws.State == WebSocketState.Open)
        {
            var result = await _ws.ReceiveAsync(buffer, ct);
            if (result.MessageType == WebSocketMessageType.Close) break;

            var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
            // تطبیق پیام ورودی به MarketTickDto باید بر اساس provider انجام شود
            var tick = JsonSerializer.Deserialize<MarketTickDto>(msg);
            if (tick != null) await onTick(tick);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_ws != null)
        {
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None);
            _ws.Dispose();
        }
    }
}