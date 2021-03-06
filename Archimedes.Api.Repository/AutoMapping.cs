﻿using Archimedes.Library.Message.Dto;
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

            CreateMap<Candle, CandleDto>();
            CreateMap<CandleDto, Candle>();

            CreateMap<Market, MarketDto>();
            CreateMap<MarketDto, Market>();

            CreateMap<PriceLevel, PriceLevelDto>();
            CreateMap<PriceLevelDto, PriceLevel>();

            CreateMap<Strategy, StrategyDto>();
            CreateMap<StrategyDto, Strategy>();
        }
    }
}