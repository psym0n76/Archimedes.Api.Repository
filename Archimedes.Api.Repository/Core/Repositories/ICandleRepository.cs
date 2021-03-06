﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Archimedes.Api.Repository
{
    public interface ICandleRepository
    {
        Task<Candle> GetCandleAsync(int id, CancellationToken ct);
        Task<List<Candle>> GetCandlesAsync(int pageIndex, int pageSize, CancellationToken ct);
        Task AddCandleAsync(Candle candle, CancellationToken ct);
        Task AddCandlesAsync(List<Candle> candles, CancellationToken ct);

        Task<List<Candle>> GetCandlesByMarketByFromDate(string market, DateTime fromDate, CancellationToken ct);
        Task<List<Candle>> GetCandlesAsync(Expression<Func<Candle, bool>> predicate, CancellationToken ct);
        void RemoveCandles(List<Candle> candle);
        Task RemoveDuplicateCandleEntries(List<Candle> candles, CancellationToken ct);
        Task<DateTime> GetLastCandleUpdated(string market, string granularity, CancellationToken ct);
        Task<DateTime> GetFirstCandleUpdated(string market, string granularity, CancellationToken ct);

        Task<int> GetCandleCount(string market, string granularity, CancellationToken ct);

        Task<List<Candle>> GetMarketGranularityCandlesDate(string market, string granularity,
            DateTimeOffset fromDate,
            DateTimeOffset toDate,
            CancellationToken ct);

        Task<List<Candle>> GetMarketGranularityCandles(string market, string granularity, CancellationToken ct);

        Task<List<Candle>> GetMarketCandles(string market, CancellationToken ct);
        Task<bool> GetCandleExists(string market, string granularity, DateTime timeStamp, CancellationToken ct);
    }
}