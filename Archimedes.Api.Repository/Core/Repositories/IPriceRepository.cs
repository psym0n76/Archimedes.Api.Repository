using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Archimedes.Library.Message.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Archimedes.Api.Repository
{
    public interface IPriceRepository : IRepository<Price>
    {
        Task<Price> GetPriceAsync(long id, CancellationToken ct);
        Task<IEnumerable<Price>> GetPricesAsync(int pageIndex, int pageSize, CancellationToken ct);
        Task<IEnumerable<Price>> GetPricesAsync(Expression<Func<Price, bool>> predicate, CancellationToken ct);
        Task AddPriceAsync(Price price);
        Task AddPricesAsync(IList<Price> prices);
        void RemovePrices(IList<Price> prices);
        Task<DateTime?> GetLastUpdated(string market, string granularity, CancellationToken ct);
        Task<IEnumerable<Price>> GetMarketGranularityPricesDate(string market, string granularity, DateTimeOffset fromDate,
            DateTimeOffset toDate,
            CancellationToken ct);
    }
}