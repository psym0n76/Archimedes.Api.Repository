using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Archimedes.Api.Repository
{
    public interface ITradeRepository
    {
        Task<Trade> GetTradeAsync(int id, CancellationToken ct);
        Task<IEnumerable<Trade>> GetTradesAsync(int pageIndex, int pageSize, CancellationToken ct);
        Task AddTradesAsync(IEnumerable<Trade> trades, CancellationToken ct);
    }
}