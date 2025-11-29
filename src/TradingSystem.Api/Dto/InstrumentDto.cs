using System;
namespace TradingSystem.Api.Dto;

public class InstrumentDto
{
    public Guid InstrumentId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Exchange { get; set; }
    public string? Currency { get; set; }
}
