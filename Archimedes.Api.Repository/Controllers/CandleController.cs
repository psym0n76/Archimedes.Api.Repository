using System.Collections.Generic;
using System.Threading.Tasks;
using Archimedes.Library.Message.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Archimedes.Api.Repository.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CandleController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly ILogger<CandleController> _logger;

        public CandleController(IMapper mapper, IUnitOfWork unit, ILogger<CandleController> logger)
        {
            _mapper = mapper;
            _unit = unit;
            _logger = logger;
        }

        // GET: api/Candle
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet()]
        public async Task<IActionResult> Get()
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = _unit.Candle.GetCandle(id);

            if (result == null)
            {
                _logger.LogError($"Candle data not found for Id: {id}");
                return NotFound();
            }

            var candleDto = _mapper.Map<CandleDto>(result);

            return Ok(candleDto);
        }
    }
}
