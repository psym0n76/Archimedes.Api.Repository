using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Archimedes.Api.Repository
{
    public class PriceRepository : Repository<Price>, IPriceRepository
    {
        public PriceRepository(DbContext context) : base(context)
        {
        }

        public ArchimedesContext FxDatabaseContext => Context as ArchimedesContext;

        public async Task<Price> GetPriceAsync(long id, CancellationToken ct)
        {
            return await FxDatabaseContext.Prices.FindAsync(id, ct);
        }

        public async Task<IEnumerable<Price>> GetPricesAsync(int pageIndex, int pageSize, CancellationToken ct)
        {
            return await FxDatabaseContext.Prices.AsNoTracking().Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Price>> GetPricesAsync(Expression<Func<Price, bool>> predicate,
            CancellationToken ct)
        {
            return await FxDatabaseContext.Prices.AsNoTracking().Where(predicate).ToListAsync(ct);
        }

        public async Task<DateTime?> GetLastUpdated(string market, string granularity, CancellationToken ct)
        {
            var response = await GetPricesAsync(a => a.Market == market && a.Granularity == granularity, ct);
            return response?.Max(a => a.Timestamp);
            
        }

        public async Task<IEnumerable<Price>> GetMarketGranularityPricesDate(string market, string granularity,
            DateTimeOffset fromDate, DateTimeOffset toDate,
            CancellationToken ct)
        {

            var prices =
                await GetPricesAsync(
                    a => a.Market == market && a.Timestamp > fromDate && a.Timestamp <= toDate &&
                         a.Granularity == granularity, ct);

            return prices;
        }

        public async Task AddPriceAsync(Price price)
        {
            await FxDatabaseContext.Prices.AddAsync(price);
        }

        public async Task AddPricesAsync(IList<Price> prices)
        {
            await FxDatabaseContext.Prices.AddRangeAsync(prices);
        }

        public void RemovePrices(IList<Price> prices)
        {
            FxDatabaseContext.Prices.RemoveRange(prices);
        }
    }
}