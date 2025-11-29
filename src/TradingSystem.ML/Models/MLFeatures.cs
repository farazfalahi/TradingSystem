using Microsoft.ML.Data;

namespace TradingSystem.ML.Models
{
    /// <summary>
    /// ساختار داده‌ای که برای آموزش و پیش‌بینی با ML.NET استفاده می‌شود.
    /// همه ویژگی‌ها باید float باشند.
    /// </summary>
    public class MLFeatures
    {
        [ColumnName("Label")]
        public float Label { get; set; } // 1 => up, 0 => down

        public float Close { get; set; }
        public float Volume { get; set; }

        public float SMA5 { get; set; }
        public float SMA20 { get; set; }

        public float RSI14 { get; set; }

        public float Momentum { get; set; }

        // در صورت نیاز فیلدهای بیشتر اضافه کن
    }
}
