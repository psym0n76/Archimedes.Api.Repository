using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Archimedes.Api.Repository
{
    public interface IPriceLevelRepository
    {
        Task<PriceLevel> GetPriceLevelAsync(int id, CancellationToken ct);
        Task<List<PriceLevel>> GetPriceLevelsAsync(int pageIndex, int pageSize, CancellationToken ct);
        Task AddPriceLevelsAsync(IEnumerable<PriceLevel> trades, CancellationToken ct);

        void RemovePriceLevels(List<PriceLevel> candle);
        Task RemoveDuplicatePriceLevelEntries(List<PriceLevel> priceLevels, CancellationToken ct);
    }
}