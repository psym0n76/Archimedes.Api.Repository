using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Archimedes.Api.Repository
{
    public interface ICandleRepository
    {
        Task<Candle> GetCandleAsync(int id, CancellationToken ct);
        Task<IEnumerable<Candle>> GetCandlesAsync(int pageIndex, int pageSize, CancellationToken ct);
        Task AddCandleAsync(Candle candle, CancellationToken ct);
        Task AddCandlesAsync(IEnumerable<Candle> candles, CancellationToken ct);
    }
}