using System.Collections.Generic;
using TradingSystem.ML.Models;

namespace TradingSystem.ML.Services
{
    public interface IFeatureEngineeringService
    {
        /// <summary>
        /// تبدیل مجموعه کندل (تاریخی) به لیستی از MLFeatures برای آموزش یا پیش‌بینی.
        /// Label باید از سوی caller تعیین شود (مثلاً آینده N کندل بالاتر/پایین).
        /// </summary>
        IEnumerable<MLFeatures> BuildFeatures(IEnumerable<CandleInput> candles);
    }
}
