using System;
namespace TradingSystem.Application.DTOs;

public record MarketTickDto(Guid InstrumentId,
                            string Symbol,
                            DateTime Timestamp,
                            decimal Open,
                            decimal High,
                            decimal Low,
                            decimal Close,
                            decimal Volume,
                            string Interval,
                            string Source,
                            int Version = 1);