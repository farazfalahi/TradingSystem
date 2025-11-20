using System;
using TradingSystem.Domain.Enums;

namespace TradingSystem.Domain.Entities
{
    public class RateLimitConfig
    {
        public Guid Id { get; private set; }
        public DataSourceType SourceType { get; private set; }
        public int RequestsPerMinute { get; private set; }
        public int BurstLimit { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public RateLimitConfig(DataSourceType sourceType, int requestsPerMinute, int burstLimit)
        {
            Id = Guid.NewGuid();
            SourceType = sourceType;
            RequestsPerMinute = requestsPerMinute;
            BurstLimit = burstLimit;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}