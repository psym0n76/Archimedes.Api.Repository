using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Archimedes.Api.Repository.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TradeController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;

        public TradeController(IMapper mapper, IUnitOfWork unit)
        {
            _mapper = mapper;
            _unit = unit;
        }

        [HttpGet(Name = "GetTrades")]
        public IActionResult Get()
        {
            var trade = _unit.Trade.GetTrades(1, 100);

            if (trade == null)
            {
                return NotFound("Trade data not found.");
            }

            var tradeDto = _mapper.Map<IEnumerable<TradeDto>>(trade);
            var json = JsonConvert.SerializeObject(tradeDto);

            return Ok(json);
        }

        // GET: api/Trade/5
        [HttpGet("{id}", Name = "GetTrade")]
        public IActionResult Get(int id)
        {
            var trade = _unit.Trade.GetTrade(id);

            if (trade == null)
            {
                return NotFound($"Trade data not found for Id: {id}");
            }

            var tradeDto = _mapper.Map<TradeDto>(trade);
            var json = JsonConvert.SerializeObject(tradeDto);

            return Ok(json);
        }

        // POST: api/Trade
        [HttpPost(Name = "PostTrade")]
        public IActionResult Post([FromBody] IEnumerable<TradeDto> value)
        {
            var trade = _mapper.Map<IEnumerable<Trade>>(value);
            _unit.Trade.AddTrades(trade);
            var records = _unit.Complete();

            return Ok($"Added {records} Trade records");
        }
    }
}