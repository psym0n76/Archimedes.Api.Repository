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
            Strategy = new StrategyRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IPriceRepository Price { get; }
        public ITradeRepository Trade { get; }
        public ICandleRepository Candle { get; }
        public IPriceLevelRepository PriceLevel { get; }
        public IStrategyRepository Strategy { get;}
        public IMarketRepository Market { get; }

        public  int SaveChanges()
        {
            return  _context.SaveChanges();
        }
    }
}