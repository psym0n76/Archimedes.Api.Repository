using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Archimedes.Api.Repository
{
    public interface IStrategyRepository
    {
        Task<Strategy> GetStrategyAsync(int id, CancellationToken ct);
        Task<List<Strategy>> GetStrategiesAsync(int pageIndex, int pageSize, CancellationToken ct);

        Task<List<Strategy>> GetActiveStrategiesGranularityMarket(string market, string granularity, CancellationToken ct);

        Task AddStrategyAsync(Strategy strategy, CancellationToken ct);
        Task AddStrategiesAsync(List<Strategy> strategy, CancellationToken ct);

        Task UpdateStrategy(Strategy strategy, CancellationToken ct);
        Task UpdateStrategyMetrics(Strategy strategy, CancellationToken ct);

        Task UpdateStrategyMaxDateAsync(int strategyId, DateTime maxDate, DateTime minDate, int candleCount,
            CancellationToken ct = default);
    }
}