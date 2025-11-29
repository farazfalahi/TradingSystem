using System;

namespace TradingSystem.ML.Models
{
    /// <summary>
    /// ورودیِ خام کندل برای مهندسی ویژگی
    /// </summary>
    public class CandleInput
    {
        public string Symbol { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }
}
