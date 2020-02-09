using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Archimedes.Api.Repository
{
    public class CandleRepository : Repository<Candle>,ICandleRepository
    {
        public CandleRepository(DbContext context) : base(context)
        {
        }

        public ArchimedesContext FxDatabaseContext => Context as ArchimedesContext;

        public Candle GetCandle(int id)
        {
            return FxDatabaseContext.Candles.Find(id);
        }

        public IEnumerable<Candle> GetCandles(int pageIndex, int pageSize)
        {
            return FxDatabaseContext.Candles.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public void AddCandle(Candle candle)
        {
            FxDatabaseContext.Candles.Add(candle);
        }

        public void AddCandles(IEnumerable<Candle> candles)
        {
            FxDatabaseContext.Candles.AddRange(candles);
        }
    }
}