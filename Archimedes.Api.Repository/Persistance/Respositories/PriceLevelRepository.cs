using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Archimedes.Api.Repository
{
    public class PriceLevelRepository : Repository<PriceLevel>, IPriceLevelRepository
    {
        public PriceLevelRepository(DbContext context) : base(context)
        {
        }

        public ArchimedesContext FxDatabaseContext => Context as ArchimedesContext;

        public async Task<PriceLevel> GetPriceLevelAsync(int id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.PriceLevels.FindAsync(id);
        }

        public async Task<List<PriceLevel>> GetPriceLevelsAsync(int pageIndex, int pageSize, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.PriceLevels.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        }

        public Task<List<PriceLevel>> GetPriceLevelsByMarketByDateAsync(string market, DateTime fromDate, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return  GetPriceLevelsAsync(a => a.Market == market && a.TimeStamp > fromDate, ct);
        }

        public Task<List<PriceLevel>> GetPriceLevelsByMarketByGranularityByDateAsync(string market, string granularity, DateTime fromDate,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return  GetPriceLevelsAsync(a => a.Market == market && a.Granularity == granularity && a.TimeStamp > fromDate, ct);
        }

        public async Task<List<PriceLevel>> GetPriceLevelsAsync(Expression<Func<PriceLevel, bool>> predicate,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.PriceLevels.AsNoTracking().Where(predicate).ToListAsync(ct);
        }

        public async Task AddPriceLevelsAsync(IEnumerable<PriceLevel> priceLevels, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.PriceLevels.AddRangeAsync(priceLevels, ct);
        }

        public async Task RemoveDuplicatePriceLevelEntries(List<PriceLevel> priceLevel, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var granularity = priceLevel.Select(a => a.Granularity).FirstOrDefault();
            var market = priceLevel.Select(a => a.Market).FirstOrDefault();

            var historicPriceLevels =
                await GetPriceLevelsAsync(a => a.Granularity == granularity && a.Market == market, ct);

            if (historicPriceLevels.Any())
            {
                var duplicatedPriceLevels = historicPriceLevels.Join(priceLevel, hist => hist.TimeStamp, current => current.TimeStamp,
                    (hist, current) => new PriceLevel()
                    {
                        Id = hist.Id,
                        Market = hist.Market,
                        Granularity = hist.Granularity,
                        TimeStamp = hist.TimeStamp,
                        LastUpdated = hist.LastUpdated,
                        BuySell = hist.BuySell,
                        BidPrice = hist.BidPrice,
                        BidPriceRange = hist.BidPriceRange,
                        AskPrice = hist.AskPrice,
                        AskPriceRange = hist.AskPriceRange,
                        Strategy = hist.Strategy,
                        Active = hist.Active,
                        CandleType = hist.CandleType
                    }).ToList();

                RemovePriceLevels(duplicatedPriceLevels);
                FxDatabaseContext.SaveChanges();
            }
        }


        public void RemovePriceLevels(List<PriceLevel> priceLevels)
        {
            FxDatabaseContext.PriceLevels.RemoveRange(priceLevels);
        }
    }
}