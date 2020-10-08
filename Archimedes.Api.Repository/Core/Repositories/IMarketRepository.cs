using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Archimedes.Api.Repository
{
    public interface IMarketRepository
    {
        Task<Market> GetMarketAsync(int id, CancellationToken ct);
        Task<IEnumerable<Market>> GetMarketsAsync(int pageIndex, int pageSize, CancellationToken ct);
        Task AddMarketAsync(Market market, CancellationToken ct);
        Task AddMarketsAsync(IEnumerable<Market> markets, CancellationToken ct);

        Task UpdateMarket(Market market, CancellationToken ct);

        Task UpdateMarketMaxDateAsync(int marketId, DateTime maxDate, DateTime minDate, int candleCount,
            CancellationToken ct = default);
    }
}