using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Archimedes.Api.Repository
{
    public class CandleRepository : Repository<Candle>, ICandleRepository
    {
        public CandleRepository(DbContext context) : base(context)
        {
        }

        public ArchimedesContext FxDatabaseContext => Context as ArchimedesContext;

        public async Task<Candle> GetCandleAsync(int id, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Candles.FindAsync(id);
        }

        public async Task<List<Candle>> GetMarketCandles(string market, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var candles =
                await GetCandlesAsync(a => a.Market == market, ct);

            return candles;
        }

        public async Task<bool> GetCandleExists(string market, string granularity, DateTime timeStamp, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var candle = await GetCandlesAsync(a =>
                a.Market == market && a.Granularity == granularity && a.TimeStamp == timeStamp,ct);

            return candle.Any();
        }

        public async Task<List<Candle>> GetCandlesAsync(int pageIndex, int pageSize,
            CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Candles.OrderBy(a=>a.TimeStamp).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        }

        public async Task<List<Candle>> GetCandlesByMarketByFromDate(string market, DateTime fromDate,
            CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Candles.AsNoTracking()
                .Where(a => a.Market == market && a.TimeStamp > fromDate).ToListAsync(ct);
        }

        public async Task<List<Candle>> GetCandlesAsync(Expression<Func<Candle, bool>> predicate,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Candles.AsNoTracking().Where(predicate).ToListAsync(ct);
        }


        public async Task AddCandleAsync(Candle candle, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.Candles.AddAsync(candle, ct);
        }

        public async Task AddCandlesAsync(List<Candle> candles, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.Candles.AddRangeAsync(candles, ct);
        }

        public async Task<DateTime> GetLastCandleUpdated(string market, string granularity, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var response = await GetCandlesAsync(a => a.Market == market && a.Granularity == granularity, ct);
            return response.Max(a => a.FromDate);
        }

        public async Task<int> GetCandleCount(string market, string granularity, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var response = await GetCandlesAsync(a => a.Market == market && a.Granularity == granularity, ct);
            return response.Count();
        }

        public async Task<DateTime> GetFirstCandleUpdated(string market, string granularity, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var response = await GetCandlesAsync(a => a.Market == market && a.Granularity == granularity, ct);
            return response.Min(a => a.FromDate);

        }

        public async Task<List<Candle>> GetMarketGranularityCandlesDate(string market, string granularity,
            DateTimeOffset fromDate, DateTimeOffset toDate,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var candles =
                await GetCandlesAsync(
                    a => a.Market == market && a.FromDate > fromDate && a.ToDate <= toDate &&
                         a.Granularity == granularity, ct);

            return candles;
        }

        public void RemoveCandles(List<Candle> candles)
        {
            FxDatabaseContext.Candles.RemoveRange(candles);
        }

        public async Task RemoveDuplicateCandleEntries(List<Candle> candle, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var granularity = candle.Select(a => a.Granularity).FirstOrDefault();
            var market = candle.Select(a => a.Market).FirstOrDefault();

            var historicCandles =
                await GetCandlesAsync(a => a.Granularity == granularity && a.Market == market, ct);

            if (historicCandles.Any())
            {

                var duplicatedCandles = historicCandles.Join(candle, hist => hist.FromDate, current => current.FromDate,
                    (hist, current) => new Candle
                    {
                        Id = hist.Id,
                        Market = hist.Market,
                        Granularity = hist.Granularity,
                        AskOpen = hist.AskOpen,
                        AskHigh = hist.AskHigh,
                        AskLow = hist.AskLow,
                        AskClose = hist.AskClose,
                        BidOpen = hist.BidOpen,
                        BidHigh = hist.BidHigh,
                        BidLow = hist.BidLow,
                        BidClose = hist.BidClose,
                        FromDate = hist.FromDate,
                        ToDate = hist.ToDate
                    }).ToList();

                RemoveCandles(duplicatedCandles);
                await FxDatabaseContext.SaveChangesAsync(ct);
            }
        }

        public async Task<List<Candle>> GetMarketGranularityCandles(string market, string granularity,
            CancellationToken ct)
        {

            ct.ThrowIfCancellationRequested();

           return await GetCandlesAsync(a => a.Market == market && a.Granularity == granularity, ct);
        }
    }
}