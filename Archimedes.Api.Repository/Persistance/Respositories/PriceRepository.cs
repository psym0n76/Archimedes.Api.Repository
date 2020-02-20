using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Archimedes.Api.Repository
{
    public class PriceRepository : Repository<Price>,IPriceRepository
    {
        public PriceRepository(DbContext context) : base(context)
        {
        }
        public ArchimedesContext FxDatabaseContext => Context as ArchimedesContext;

        public async Task<Price> GetPrice(long id)
        {
            return await FxDatabaseContext.Prices.FindAsync(id);
        }

        public async Task <IEnumerable<Price>> GetPrices(int pageIndex, int pageSize)
        {
            return await FxDatabaseContext.Prices.AsNoTracking().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task< IEnumerable<Price>> GetPrices(Expression<Func<Price, bool>> predicate)
        {
            return await FxDatabaseContext.Prices.AsNoTracking().Where(predicate).ToListAsync();
        }

        public void AddPrice(Price price)
        {
            FxDatabaseContext.Prices.AddAsync(price);
        }

        public void AddPrices(IEnumerable<Price> prices)
        {
            FxDatabaseContext.Prices.AddRangeAsync(prices);
        }

        public void RemovePrices(IEnumerable<Price> prices)
        {
            FxDatabaseContext.Prices.RemoveRange(prices);
        }
    }
}