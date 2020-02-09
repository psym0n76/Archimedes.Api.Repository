using System.Collections.Generic;

namespace Archimedes.Api.Repository
{
    public interface IPriceRepository
    {
        Price GetPrice(long id);
        IEnumerable<Price> GetPrices(int pageIndex, int pageSize);
        void AddPrice(Price price);
        void AddPrices(IEnumerable<Price> prices);
    }
}