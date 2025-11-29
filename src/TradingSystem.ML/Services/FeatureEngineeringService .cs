using System;
using System.Collections.Generic;
using System.Linq;
using Skender.Stock.Indicators;
using TradingSystem.ML.Models;

namespace TradingSystem.ML.Services
{
    /// <summary>
    /// استفاده از Skender.Stock.Indicators برای محاسبه SMA/RSI و تولید ویژگی‌ها
    /// </summary>
    public class FeatureEngineeringService : IFeatureEngineeringService
    {
        /// <summary>
        /// ورودی: مجموعه کندل‌ها (ترتیب صعودی بر اساس زمان)
        /// خروجی: MLFeatures به ترتیب متناظر (اولین اندیکاتورها تا زمانی که قابلیت محاسبه وجود داشته باشد null فیلتر می‌شوند)
        /// </summary>
        public IEnumerable<MLFeatures> BuildFeatures(IEnumerable<CandleInput> candles)
        {
            if (candles == null) throw new ArgumentNullException(nameof(candles));

            var list = candles.Select(c => new Quote
            {
                Date = c.Timestamp,
                Open = c.Open,
                High = c.High,
                Low = c.Low,
                Close = c.Close,
                Volume = c.Volume
            }).ToList();

            // محاسبه اندیکاتورها
            var sma5 = list.GetSma(5).ToList();
            var sma20 = list.GetSma(20).ToList();
            var rsi14 = list.GetRsi(14).ToList();

            // برای دسترسی سریع توسط date استفاده می‌کنیم
            var sma5ByDate = sma5.ToDictionary(x => x.Date, x => x.Sma);
            var sma20ByDate = sma20.ToDictionary(x => x.Date, x => x.Sma);
            var rsiByDate = rsi14.ToDictionary(x => x.Date, x => x.Rsi);

            // تولید MLFeatures — حذف مواردی که اندیکاتور ندارند
            foreach (var c in candles)
            {
                if (!sma5ByDate.TryGetValue(c.Timestamp, out var s5)) continue;
                if (!sma20ByDate.TryGetValue(c.Timestamp, out var s20)) continue;
                if (!rsiByDate.TryGetValue(c.Timestamp, out var rsi)) continue;

                // momentum = close - sma5 (نمونه ساده)
                var momentum = (float)((double)c.Close - s5);

                yield return new MLFeatures
                {
                    // Label باید توسط caller تنظیم شود یا بعداً تعیین شود (اینجا صفر پیش‌فرض)
                    Label = 0f,
                    Close = (float)c.Close,
                    Volume = (float)c.Volume,
                    SMA5 = (float)s5,
                    SMA20 = (float)s20,
                    RSI14 = (float)rsi,
                    Momentum = momentum
                };
            }
        }
    }
}
