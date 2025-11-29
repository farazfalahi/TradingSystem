namespace TradingSystem.ML.Models
{
    public class PriceInput
    {
        public string Symbol { get; set; }
        public int LastPrice { get; set; }
        public int Volume { get; set; }
        public int RSI { get; set; }
        public int SMA20 { get; set; }
        public int SMA50 { get; set; }
    }
}