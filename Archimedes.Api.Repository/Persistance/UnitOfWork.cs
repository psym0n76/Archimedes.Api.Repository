
using System.Threading.Tasks;

namespace Archimedes.Api.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ArchimedesContext _context;
        public UnitOfWork(ArchimedesContext context)
        {
            _context = context;
            Price = new PriceRepository(_context);
            Candle = new CandleRepository(_context);
            Trade = new TradeRepository(_context);
            Market = new MarketRepository(_context);
            PriceLevel = new PriceLevelRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IPriceRepository Price { get; }
        public ITradeRepository Trade { get; }
        public ICandleRepository Candle { get; set; }
        public IPriceLevelRepository PriceLevel { get; set; }

        public IMarketRepository Market { get; set; }
        public  int SaveChanges()
        {
            return  _context.SaveChanges();
        }
    }
}