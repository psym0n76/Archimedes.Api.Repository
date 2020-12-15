using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Archimedes.Api.Repository
{
    public interface IPriceLevelRepository
    {
        Task<PriceLevel> GetPriceLevelAsync(int id, CancellationToken ct);
        Task<List<PriceLevel>> GetPriceLevelsAsync(int pageIndex, int pageSize, CancellationToken ct);

        Task<List<PriceLevel>> GetPriceLevelsByTimeStamp(DateTime timestamp, string granularity, string strategy, DateTime fromDate, CancellationToken ct);
        Task<List<PriceLevel>> GetPriceLevelsByMarketByDateAsync(string market, DateTime fromDate, CancellationToken ct);
        Task<List<PriceLevel>> GetPriceLevelsByMarketByGranularityByDateAsync(string market, string granularity, DateTime fromDate, CancellationToken ct);

        Task AddPriceLevelsAsync(IEnumerable<PriceLevel> priceLevel, CancellationToken ct);

        Task<List<PriceLevel>> RemoveDuplicatePriceLevelEntries(List<PriceLevel> priceLevel, CancellationToken ct);

        Task UpdatePriceLevelAsync(PriceLevel priceLevel, CancellationToken ct);
    }
}