using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TradingSystem.Infrastructure.Repositories;
namespace TradingSystem.DataCollector.Services;

public class Bootstrapper
{
    private readonly IHttpMarketClient _httpClient;
    private readonly IInstrumentRepository _instruments; // از Infrastructure
    private readonly int _parallelism;

    public Bootstrapper(IHttpMarketClient httpClient, IInstrumentRepository instruments, int parallelism = 8)
    {
        _httpClient = httpClient;
        _instruments = instruments;
        _parallelism = parallelism;
    }

    public async Task BootstrapFromFileAsync(string csvPath, CancellationToken ct = default)
    {
        var symbols = File.ReadAllLines(csvPath);
        var tasks = new List<Task>();
        using var sem = new SemaphoreSlim(_parallelism);

        foreach (var s in symbols)
        {
            await sem.WaitAsync(ct);
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    // نمونه: دانلود تاریخچه 1 سال
                    var to = DateTime.UtcNow;
                    var from = to.AddYears(-1);
                    var candles = await _httpClient.GetHistoricalCandlesAsync(s, from, to, ct);
                    // تبدیل و ذخیره در repository
                    await _instruments.SaveHistoricalCandlesAsync(s, candles);
                }
                finally { sem.Release(); }
            }, ct));
        }

        await Task.WhenAll(tasks);
    }
}