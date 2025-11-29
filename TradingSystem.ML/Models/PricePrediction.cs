namespace TradingSystem.ML.Models
{
    public class PricePrediction
    {
        public bool PredictedLabel { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
