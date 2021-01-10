using System;
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
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Trades.FindAsync(id);
        }

        public async Task<List<Trade>> GetTradesAsync(int pageIndex, int pageSize, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.Trades.OrderBy(a=>a.TimeStamp).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        }


        public async Task UpdateTradeAsync(Trade tradeDto, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var trade = await FxDatabaseContext.Trades.FindAsync(tradeDto.Id);

            if (trade == null)
            {
                throw new ArgumentNullException(nameof(trade), $"Unable to find {tradeDto.Id}");
            }

            trade.Success = tradeDto.Success;
            trade.Price = tradeDto.Price;
            trade.RiskReward = tradeDto.RiskReward;
            trade.LastUpdated = DateTime.Now;

            FxDatabaseContext.Trades.Update(trade);
        }


        public async Task AddTradesAsync(IEnumerable<Trade> trades, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.Trades.AddRangeAsync(trades, ct);
        }
    }
}