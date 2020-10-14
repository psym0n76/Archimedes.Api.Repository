using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<PriceLevel>> GetPriceLevelsAsync(int pageIndex, int pageSize, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await FxDatabaseContext.PriceLevels.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        }

        public async Task AddPriceLevelsAsync(IEnumerable<PriceLevel> priceLevels, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            await FxDatabaseContext.PriceLevels.AddRangeAsync(priceLevels, ct);
        }
    }
}