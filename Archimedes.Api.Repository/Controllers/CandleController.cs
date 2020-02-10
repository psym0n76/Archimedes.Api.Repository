using System.Collections.Generic;
using Archimedes.Library.Message.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Archimedes.Api.Repository.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CandleController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        public CandleController(IMapper mapper, IUnitOfWork unit)
        {
            _mapper = mapper;
            _unit = unit;
        }

        // GET: api/Candle
        [HttpGet(Name = "GetCandles")]
        public IActionResult Get()
        {
            var candle = _unit.Candle.GetCandles(1, 100);

            if (candle == null)
            {
                return NotFound("Candle data not found.");
            }

            var candleDto = _mapper.Map<IEnumerable<CandleDto>>(candle);
            var json = JsonConvert.SerializeObject(candleDto);

            return Ok(json);
        }

        // GET: api/Candle/5
        [HttpGet("{id}", Name = "GetCandle")]
        public IActionResult Get(int id)
        {
            var result = _unit.Candle.GetCandle(id);

            if (result == null)
            {
                return NotFound($"Candle data not found for Id: {id}");
            }

            var priceDto = _mapper.Map<PriceDto>(result);
            var json = JsonConvert.SerializeObject(priceDto);

            return Ok(json);
        }
    }
}
