using Microsoft.ML.Data;

namespace TradingSystem.ML.Models;

public class PredictionOutput
{
    [ColumnName("Score")]
    public float Score { get; set; }
}