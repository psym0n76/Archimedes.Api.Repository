using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Archimedes.Api.Repository
{
    public class TradeRepository : Repository<Trade>, ITradeRepository
    {
        public TradeRepository(DbContext context) : base(context)
        {
        }

        public ArchimedesContext FxDatabaseContext => Context as ArchimedesContext;

        public async Task<Trade> GetTradeAsync(int id, CancellationToken ct)
        {
            return await FxDatabaseContext.Trades.FindAsync(id, ct);
        }

        public async Task<IEnumerable<Trade>> GetTradesAsync(int pageIndex, int pageSize, CancellationToken ct)
        {
            return await FxDatabaseContext.Trades.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        }

        public async Task AddTradeAsync(Trade trade, CancellationToken ct)
        {
            await FxDatabaseContext.Trades.AddAsync(trade, ct);
        }

        public async Task AddTradesAsync(IEnumerable<Trade> trades, CancellationToken ct)
        {
            await FxDatabaseContext.Trades.AddRangeAsync(trades, ct);
        }
    }
}