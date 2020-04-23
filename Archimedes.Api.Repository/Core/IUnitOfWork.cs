using System;
using System.Threading.Tasks;

namespace Archimedes.Api.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IPriceRepository Price { get; }
        ICandleRepository Candle { get; }
        ITradeRepository Trade { get; }
        Task<int> SaveChangesAsync();
    }
}