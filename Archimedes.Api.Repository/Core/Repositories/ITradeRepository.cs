using System.Collections.Generic;

namespace Archimedes.Api.Repository
{
    public interface ITradeRepository
    {
        Trade GetTrade(int id);
        IEnumerable<Trade> GetTrades(int pageIndex, int pageSize);
        void AddTrade(Trade candle);
        void AddTrades(IEnumerable<Trade> candles);
    }
}