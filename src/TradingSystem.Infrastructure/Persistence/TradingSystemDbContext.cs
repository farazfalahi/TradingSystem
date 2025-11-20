using Microsoft.EntityFrameworkCore;
using TradingSystem.Domain.Entities;

namespace TradingSystem.Infrastructure.Persistence;

public class TradingSystemDbContext : DbContext
{
    public TradingSystemDbContext(DbContextOptions<TradingSystemDbContext> options) : base(options) { }

    public DbSet<Instrument> Instruments { get; set; }
    public DbSet<MarketData> MarketData { get; set; }
    public DbSet<RateLimitConfig> RateLimitConfigs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Instrument>()
            .HasKey(i => i.Id);

        modelBuilder.Entity<Instrument>()
            .HasIndex(i => i.Symbol)
            .IsUnique();

        modelBuilder.Entity<MarketData>()
            .HasKey(md => md.Id);

        modelBuilder.Entity<MarketData>()
            .HasOne<Instrument>()
            .WithMany()
            .HasForeignKey(md => md.InstrumentId);

        modelBuilder.Entity<RateLimitConfig>()
            .HasKey(r => r.Id);
    }
}