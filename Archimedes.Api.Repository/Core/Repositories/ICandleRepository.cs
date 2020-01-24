using System.Collections.Generic;

namespace Archimedes.Fx.Api.Repository
{
    public interface ICandleRepository : IRepository<Candle>
    {
        Candle GetCandle(int id);
        IEnumerable<Candle> GetCandles(int pageIndex, int pageSize);
        void AddCandle(Candle candle);
        void AddCandles(IEnumerable<Candle> candles);
    }
}