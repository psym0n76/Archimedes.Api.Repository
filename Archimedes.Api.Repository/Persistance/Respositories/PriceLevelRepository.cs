using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
            return await FxDatabaseContext.PriceLevels.OrderBy(a=>a.TimeStamp).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            //return await FxDatabaseContext.PriceLevels.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        }

        public Task<List<PriceLevel>> GetPriceLevelsByTimeStamp(DateTime timestamp, string granularity, string strategy, DateTime fromDate,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return GetPriceLevelsAsync(a => a.TimeStamp == timestamp && a.Granularity == granularity && a.Strategy == strategy, ct);
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

        public Task<List<PriceLevel>> GetPriceLevelsByMarketByGranularityByDateActiveAsync(string market,
            string granularity, DateTime fromDate,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return GetPriceLevelsAsync(
                a => a.Market == market && a.Granularity == granularity && a.TimeStamp > fromDate && a.Active, ct);
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

        public async Task UpdatePriceLevelAsync(PriceLevel priceLevel, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var level = await FxDatabaseContext.PriceLevels.FindAsync(priceLevel.Id);

            if (level==null)
            {
                throw new ArgumentNullException(nameof(priceLevel), $"Unable to find {priceLevel.Id}");
            }

            level.Active = priceLevel.Active;
            level.Trade = priceLevel.Trade;
            level.Trades = priceLevel.Trades;
            level.LevelBrokenDate = priceLevel.LevelBrokenDate;
            level.CandlesElapsedLevelBroken = priceLevel.CandlesElapsedLevelBroken;
            level.LastUpdated = priceLevel.LastUpdated;
            level.LevelsBroken = priceLevel.LevelsBroken;
            level.LevelBroken = priceLevel.LevelBroken;
            level.LevelExpired = priceLevel.LevelExpired;
            level.LevelExpiredDate = priceLevel.LevelExpiredDate;
            level.OutsideRange = priceLevel.OutsideRange;
            level.OutsideRangeDate = priceLevel.OutsideRangeDate;

            FxDatabaseContext.PriceLevels.Update(level);
        }

        public async Task<List<PriceLevel>> RemoveDuplicatePriceLevelEntries(List<PriceLevel> priceLevel, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var granularity = priceLevel[0].Granularity;
            var market = priceLevel[0].Market;
            //var strategy = priceLevel[0].Strategy;
            var minTimeStamp = priceLevel.Min(a => a.TimeStamp);
            

            var historicPriceLevels =
                await GetPriceLevelsAsync(a => a.Granularity == granularity && a.Market == market && a.TimeStamp >= minTimeStamp, ct);

            var confirmedPriceLevel = new List<PriceLevel>();
            
            if (!historicPriceLevels.Any()) return priceLevel;
            {
                
                foreach (var level in priceLevel)
                {
                    if (historicPriceLevels.Exists(a => a.TimeStamp == level.TimeStamp && a.BuySell == level.BuySell && a.Strategy == level.Strategy))
                    {
                    }
                    else
                    {
                        confirmedPriceLevel.Add(level);
                    }
                }
            }

            return confirmedPriceLevel;
        }
    }
}