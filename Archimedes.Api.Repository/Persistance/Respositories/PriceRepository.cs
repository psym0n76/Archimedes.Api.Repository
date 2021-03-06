﻿using System;
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
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Prices.FindAsync(id);
        }

        public async Task<List<Price>> GetPricesAsync(int pageIndex, int pageSize, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Prices.AsNoTracking().OrderBy(a=>a.TimeStamp).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            //return await FxDatabaseContext.Prices.AsNoTracking().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        }

        public async Task<List<Price>> GetPricesAsync(Expression<Func<Price, bool>> predicate,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Prices.AsNoTracking().Where(predicate).ToListAsync(ct);
        }

        public async Task RemovePricesOlderThanOneHour(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var prices =  await GetPricesAsync(a => a.TimeStamp < DateTime.Now.AddHours(-1), ct);
            RemovePrices(prices);
            await FxDatabaseContext.SaveChangesAsync(ct);
        }

        public async Task<DateTime?> GetLastUpdated(string market, string granularity, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var response = await GetPricesAsync(a => a.Market == market && a.Granularity == granularity, ct);
            return response?.Max(a => a.TimeStamp);
        }



        public async Task<List<Price>> GetMarketPrices(string market, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var prices =
                await GetPricesAsync(a => a.Market == market, ct);

            return prices;
        }

        public async Task<Price> GetLastPriceByMarket(string market, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            
            var prices =
                await GetPricesAsync(a => a.Market == market, ct);

            var recentPrice = prices.OrderByDescending(a => a.TimeStamp).Take(1).Single();

            return recentPrice;
        }

        public async Task<List<Price>> GetMarketGranularityPricesDate(string market, string granularity,
            DateTimeOffset fromDate, DateTimeOffset toDate,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var prices =
                await GetPricesAsync(
                    a => a.Market == market && a.TimeStamp > fromDate && a.TimeStamp <= toDate &&
                         a.Granularity == granularity, ct);

            return prices;
        }

        public async Task AddPriceAsync(Price price, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.Prices.AddAsync(price, ct);
        }

        public async Task AddPricesAsync(List<Price> prices, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.Prices.AddRangeAsync(prices, ct);
        }


        public void RemovePrices(List<Price> prices)
        {
            FxDatabaseContext.Prices.RemoveRange(prices);
        }

        public async Task RemoveDuplicatePriceEntries(List<Price> price, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var granularity = price.Select(a => a.Granularity).FirstOrDefault();
            var market = price.Select(a => a.Market).FirstOrDefault();

            var historicPrices =
                await GetPricesAsync(a => a.Granularity == granularity && a.Market == market, ct);

            if (historicPrices.Any())
            {

                var duplicatedPrices = historicPrices.Join(price, hist => hist.TimeStamp, current => current.TimeStamp,
                    (hist, current) => new Price
                    {
                        Id = hist.Id,
                        Market = hist.Market,
                        Granularity = hist.Granularity,
                        Ask = hist.Bid,
                        Bid = hist.Ask,
                        TimeStamp = hist.TimeStamp
                    }).ToList();

                RemovePrices(duplicatedPrices);
                await FxDatabaseContext.SaveChangesAsync(ct);
            }
        }
    }
}