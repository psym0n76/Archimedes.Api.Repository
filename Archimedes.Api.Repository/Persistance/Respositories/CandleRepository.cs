using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<Candle>> GetCandlesAsync(int pageIndex, int pageSize,
            CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Candles.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        }

        public async Task AddCandleAsync(Candle candle, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.Candles.AddAsync(candle, ct);
        }

        public async Task AddCandlesAsync(IEnumerable<Candle> candles, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.Candles.AddRangeAsync(candles, ct);
        }
    }
}