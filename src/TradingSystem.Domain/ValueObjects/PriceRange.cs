using System;

namespace TradingSystem.Domain.ValueObjects
{
    public class PriceRange
    {
        public decimal High { get; private set; }
        public decimal Low { get; private set; }

        public PriceRange(decimal low, decimal high)
        {
            if (low > high) throw new Exceptions.DomainException("Low cannot be greater than High");
            Low = low;
            High = high;
        }
    }
}