using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Archimedes.Library.Message.Dto;

namespace Archimedes.Api.Repository
{
    public interface IPriceRepository : IRepository<Price>
    {
        Task<Price> GetPriceAsync(long id, CancellationToken ct);
        Task<IList<Price>> GetPricesAsync(int pageIndex, int pageSize, CancellationToken ct);
        Task<IList<Price>> GetPricesAsync(Expression<Func<Price, bool>> predicate, CancellationToken ct);
        Task RemovePricesOlderThanOneHour(CancellationToken ct);

        Task AddPriceAsync(Price price, CancellationToken ct);
        Task AddPricesAsync(IList<Price> prices, CancellationToken ct);

        void RemovePrices(IList<Price> prices);
        Task<DateTime?> GetLastUpdated(string market, string granularity, CancellationToken ct);


        Task<IEnumerable<Price>> GetMarketGranularityPricesDate(string market, string granularity,
            DateTimeOffset fromDate,
            DateTimeOffset toDate,
            CancellationToken ct);

        Task<IEnumerable<Price>> GetMarketPrices(string market, CancellationToken ct);

        Task<Price> GetLastPriceByMarket(string market, CancellationToken ct);
    }
}