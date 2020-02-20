using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Archimedes.Api.Repository
{
    public interface IPriceRepository : IRepository<Price>
    {
        Task<Price> GetPrice(long id);
        Task<IEnumerable<Price>> GetPrices(int pageIndex, int pageSize);
        Task<IEnumerable<Price>> GetPrices(Expression<Func<Price, bool>> predicate);
        void AddPrice(Price price);
        void AddPrices(IEnumerable<Price> prices);
        void RemovePrices(IEnumerable<Price> prices);
    }
}