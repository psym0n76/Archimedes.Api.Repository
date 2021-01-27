using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<Market>> GetMarketsAsync(int pageIndex, int pageSize,
            CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Markets.OrderBy(a=>a.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            //return await FxDatabaseContext.Markets.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        }

        public async Task AddMarketAsync(Market market, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.Markets.AddAsync(market, ct);
        }

        public async Task AddMarketsAsync(List<Market> markets, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.Markets.AddRangeAsync(markets, ct);
        }

        public async Task UpdateMarket(Market updated, CancellationToken ct)
        {
            var market = await GetMarketAsync(updated.Id, ct);

            market.Interval = updated.Interval;
            market.Active = updated.Active;
            market.Granularity = updated.Granularity;
            market.MaxDate = updated.MaxDate;
            market.MinDate = updated.MinDate;
            market.Name = updated.Name;
            market.Quantity = updated.Quantity;
            market.TimeFrame = updated.TimeFrame;
            market.ExternalMarketId = updated.ExternalMarketId;

            market.LastUpdated = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            FxDatabaseContext.Markets.Update(market);
        }

        public async Task<bool> UpdateMarketMetrics(Market updated, CancellationToken ct)
        {
            var market = await GetMarketAsync(updated.Id, ct);

            if (market!=null)
            {
                market.MaxDate = updated.MaxDate;
                market.MinDate = updated.MinDate;
                market.Quantity = updated.Quantity;
                market.LastUpdated = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                FxDatabaseContext.Markets.Update(market);

                return true;
            }

            return false;
        }

        public async Task<bool> UpdateMarketStatus(Market updated, CancellationToken ct)
        {
            var market = await GetMarketAsync(updated.Id, ct);

            if (market!=null)
            {
                market.Active = updated.Active;
                market.LastUpdated = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                FxDatabaseContext.Markets.Update(market);

                return true;
            }

            return false;
        }

        public async Task UpdateMarketMaxDateAsync(int marketId, DateTime maxDate, DateTime minDate, int candleCount,
            CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var market = await GetMarketAsync(marketId, ct);

            market.MaxDate = DateTime.Parse(maxDate.ToString("yyyy-MM-dd HH:mm:ss"));
            market.LastUpdated = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            market.MinDate = DateTime.Parse(minDate.ToString("yyyy-MM-dd HH:mm:ss"));;
            market.Quantity = candleCount;

            FxDatabaseContext.Markets.Update(market);
        }
    }
}