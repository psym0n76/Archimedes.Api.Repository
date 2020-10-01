using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DateTime = System.DateTime;

namespace Archimedes.Api.Repository
{
    public class MarketRepository : Repository<Market>, IMarketRepository
    {
        public MarketRepository(DbContext context) : base(context)
        {
        }

        public ArchimedesContext FxDatabaseContext => Context as ArchimedesContext;

        public async Task<Market> GetMarketAsync(int id, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Markets.FindAsync(id);
        }

        public async Task<IEnumerable<Market>> GetMarketsAsync(int pageIndex, int pageSize,
            CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Markets.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        }

        public async Task AddMarketAsync(Market market, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.Markets.AddAsync(market, ct);
        }

        public async Task AddMarketsAsync(IEnumerable<Market> markets, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.Markets.AddRangeAsync(markets, ct);
        }

        public async Task UpdateMarketMaxDateAsync(int marketId, DateTime maxDate, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var market = await GetMarketAsync(marketId, ct);

            market.MaxDate = maxDate;
            market.LastUpdated = DateTime.Now;

            FxDatabaseContext.Markets.Update(market);
        }
    }
}