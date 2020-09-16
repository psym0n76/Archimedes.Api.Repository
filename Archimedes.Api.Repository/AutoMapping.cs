using Archimedes.Library.Message.Dto;
using AutoMapper;

namespace Archimedes.Api.Repository
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Price, PriceDto>();
            CreateMap<Trade, TradeDto>();
            CreateMap<Candle, CandleDto>();
            CreateMap<Market, MarketDto>();
        }
    }
}