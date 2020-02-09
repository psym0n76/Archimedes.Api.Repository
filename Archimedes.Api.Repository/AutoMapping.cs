using Archimedes.Api.Repository.DTO;
using AutoMapper;

namespace Archimedes.Api.Repository
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Price, PriceDto>();
            CreateMap<PriceDto, Price>();
            CreateMap<Trade, TradeDto>();
            CreateMap<TradeDto, Trade>();
        }
    }
}