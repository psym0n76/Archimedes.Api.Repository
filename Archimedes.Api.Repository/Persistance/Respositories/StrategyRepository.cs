using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Archimedes.Api.Repository
{
    public class StrategyRepository : Repository<Strategy>, IStrategyRepository
    {
        public StrategyRepository(DbContext context) : base(context)
        {
        }

        public ArchimedesContext FxDatabaseContext => Context as ArchimedesContext;

        public async Task<Strategy> GetStrategyAsync(int id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Strategy.FindAsync(id);
        }

        public async Task<List<Strategy>> GetStrategiesAsync(Expression<Func<Strategy, bool>> predicate,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Strategy.AsNoTracking().Where(predicate).ToListAsync(ct);
        }

        public async Task<IEnumerable<Strategy>> GetStrategiesAsync(int pageIndex, int pageSize, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Strategy.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        }

        public async Task<IEnumerable<Strategy>> GetActiveStrategiesGranularityMarket(string market, string granularity,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await GetStrategiesAsync(a => a.Market == market && a.Granularity == granularity && a.Active, ct);
        }

        public async Task AddStrategyAsync(Strategy strategy, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.Strategy.AddAsync(strategy, ct);
        }

        public async Task AddStrategiesAsync(IEnumerable<Strategy> strategy, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.Strategy.AddRangeAsync(strategy, ct);
        }

        public async Task UpdateStrategy(Strategy strategy, CancellationToken ct)
        {
            var existing = await GetStrategyAsync(strategy.Id, ct);

            existing.Name = strategy.Name;
            existing.Market = strategy.Market;
            existing.Granularity = strategy.Granularity;
            existing.Active = strategy.Active;

            existing.StartDate = strategy.StartDate;
            existing.EndDate = strategy.EndDate;

            existing.Count = strategy.Count;

            existing.LastUpdated = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            FxDatabaseContext.Strategy.Update(existing);
        }

        public Task UpdateStrategyMetrics(Strategy strategy, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task UpdateStrategyMaxDateAsync(int strategyId, DateTime maxDate, DateTime minDate, int candleCount,
            CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}