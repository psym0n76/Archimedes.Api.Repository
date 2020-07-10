using System;

namespace Archimedes.Api.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IPriceRepository Price { get; }
        ICandleRepository Candle { get; }
        ITradeRepository Trade { get; }
        IMarketRepository Market { get; }
        int SaveChanges();
    }
}