using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Archimedes.Api.Repository
{
    public interface IPriceRepository : IRepository<Price>
    {
        Price GetPrice(long id);
        IEnumerable<Price> GetPrices(int pageIndex, int pageSize);
        IEnumerable<Price> GetPrices(Expression<Func<Price, bool>> predicate);
        void AddPrice(Price price);
        void AddPrices(IEnumerable<Price> prices);
        void RemovePrices(IEnumerable<Price> prices);
    }
}