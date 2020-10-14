using Microsoft.EntityFrameworkCore;

namespace Archimedes.Api.Repository
{
    public class ArchimedesContext : DbContext
    {
        public DbSet<Price> Prices { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Candle> Candles { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<PriceLevel> PriceLevels { get; set; }
        public ArchimedesContext(DbContextOptions<ArchimedesContext> options) : base(options)
        {
        }
    }
}