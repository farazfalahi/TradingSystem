using System;
namespace TradingSystem.Application.DTOs;

public class SymbolDto
{
    public string Symbol { get; set; }
    public string Name { get; set; }
    public decimal LastPrice { get; set; }
    public DateTime UpdatedAt { get; set; }
}