using System;

namespace Archimedes.Api.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        //list each tables interface
        IPriceRepository Price { get; }
        ICandleRepository Candle { get; }
        ITradeRepository Trade { get; }

        int Complete();
    }
}