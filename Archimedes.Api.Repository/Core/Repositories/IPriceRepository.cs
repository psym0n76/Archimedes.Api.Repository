using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Archimedes.Api.Repository
{
    public interface IPriceRepository : IRepository<Price>
    {
        Task<Price> GetPriceAsync(long id, CancellationToken ct);
        Task<List<Price>> GetPricesAsync(int pageIndex, int pageSize, CancellationToken ct);
        Task<List<Price>> GetPricesAsync(Expression<Func<Price, bool>> predicate, CancellationToken ct);
        Task RemovePricesOlderThanOneHour(CancellationToken ct);

        Task AddPriceAsync(Price price, CancellationToken ct);
        Task AddPricesAsync(List<Price> prices, CancellationToken ct);

        void RemovePrices(List<Price> prices);
        Task<DateTime?> GetLastUpdated(string market, string granularity, CancellationToken ct);


        Task<List<Price>> GetMarketGranularityPricesDate(string market, string granularity,
            DateTimeOffset fromDate,
            DateTimeOffset toDate,
            CancellationToken ct);

        Task<List<Price>> GetMarketPrices(string market, CancellationToken ct);

        Task<Price> GetLastPriceByMarket(string market, CancellationToken ct);
    }
}