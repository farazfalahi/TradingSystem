using System;
using Microsoft.ML.Data;
namespace TradingSystem.ML.Models;

public class TradingData
{
    [LoadColumn(0), ColumnName("Label")]
    public float Label { get; set; }

    [LoadColumn(1)]
    public float Close { get; set; }

    [LoadColumn(2)]
    public float Volume { get; set; }

    [LoadColumn(3)]
    public float SMA_5 { get; set; }

    [LoadColumn(4)]
    public float SMA_20 { get; set; }

    [LoadColumn(5)]
    public float RSI_14 { get; set; }

    [LoadColumn(6)]
    public float Momentum { get; set; }
}