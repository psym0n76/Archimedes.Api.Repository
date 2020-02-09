using Archimedes.Library.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Archimedes.Api.Repository
{
    public class ArchimedesContext : DbContext
    {
        public DbSet<Price> Prices { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Candle> Candles { get; set; }


        private readonly Config  _config;

        //public ArchimedesContext(IOptions<Config> configuration)
        //{
        //    _config = configuration.Value;
        //}

        public ArchimedesContext(DbContextOptions<ArchimedesContext> options) : base(options)
        {

        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(_config.DatabaseServerConnection);
        //}
    }
}