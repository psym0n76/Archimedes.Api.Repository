using System.Collections.Generic;
using Archimedes.Library.Message.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Archimedes.Api.Repository.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CandleController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly ILogger<CandleController> _logger;
        public CandleController(IMapper mapper, IUnitOfWork unit,ILogger<CandleController> logger)
        {
            _mapper = mapper;
            _unit = unit;
            _logger = logger;
        }

        // GET: api/Candle
        [HttpGet(Name = "GetCandles")]
        public IActionResult Get()
        {
            _logger.LogInformation("Get Candles");
            var candle = _unit.Candle.GetCandles(1, 100);

            if (candle == null)
            {
                _logger.LogError("Candle data not found.");
                return NotFound();

            }

            var candleDto = _mapper.Map<IEnumerable<CandleDto>>(candle);

            return Ok(candleDto);
        }

        // GET: api/Candle/5
        [HttpGet("{id}", Name = "GetCandle")]
        public IActionResult Get(int id)
        {
            var result = _unit.Candle.GetCandle(id);

            if (result == null)
            {
                _logger.LogError($"Candle data not found for Id: {id}");
                return NotFound();
            }

            var candleDto = _mapper.Map<PriceDto>(result);

            return Ok(candleDto);
        }
    }
}
