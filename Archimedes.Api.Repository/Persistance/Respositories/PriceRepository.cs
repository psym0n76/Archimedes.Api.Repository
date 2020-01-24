using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Archimedes.Fx.Api.Repository
{
    public class PriceRepository : Repository<Price>,IPriceRepository
    {
        public PriceRepository(DbContext context) : base(context)
        {
        }
        public ArchimedesContext FxDatabaseContext => Context as ArchimedesContext;

        public Price GetPrice(int id)
        {
            return FxDatabaseContext.Prices.Find(id);
        }

        public IEnumerable<Price> GetPrices(int pageIndex, int pageSize)
        {
            return FxDatabaseContext.Prices.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public void AddPrice(Price price)
        {
            FxDatabaseContext.Prices.Add(price);
        }

        public void AddPrices(IEnumerable<Price> prices)
        {
            FxDatabaseContext.Prices.AddRange(prices);
        }
    }
}