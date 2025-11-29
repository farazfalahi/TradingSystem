using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using TradingSystem.Application.DTOs;

namespace TradingSystem.Api.Clients
{
    public interface IMarketCollectorClient
    {
        [Post("/collector/trigger")]
        Task TriggerCollectAsync([Body] TriggerRequest request);

        [Get("/collector/status")]
        Task<Dictionary<string, string>> GetStatusAsync();
    }
}
