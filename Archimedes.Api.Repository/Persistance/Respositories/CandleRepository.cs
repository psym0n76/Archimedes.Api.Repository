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

        public async Task<List<Candle>> GetCandlesAsync(int pageIndex, int pageSize,
            CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Candles.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
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

        public async Task<DateTime?> GetLastUpdated(string market, string granularity, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var response = await GetCandlesAsync(a => a.Market == market && a.Granularity == granularity, ct);
            return response?.Max(a => a.DateTo);

        }

        public async Task<List<Candle>> GetMarketGranularityCandlesDate(string market, string granularity,
            DateTimeOffset fromDate, DateTimeOffset toDate,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var candles =
                await GetCandlesAsync(
                    a => a.Market == market && a.DateFrom > fromDate && a.DateTo <= toDate &&
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

                var duplicatedCandles = historicCandles.Join(candle, hist => hist.DateFrom, current => current.DateFrom,
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
                        DateFrom = hist.DateFrom,
                        DateTo = hist.DateTo
                    }).ToList();

                RemoveCandles(duplicatedCandles);
                FxDatabaseContext.SaveChanges();
            }
        }
    }
}