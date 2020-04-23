using System.Collections.Generic;
using System.Threading;
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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<CandleDto>>> GetCandlesAsync(CancellationToken ct)
        {
            var candle = await _unit.Candle.GetCandlesAsync(1, 100, ct);

            if (candle == null)
            {
                _logger.LogError("Candle not found.");
                return NotFound();
            }

            var candleDto = _mapper.Map<IEnumerable<CandleDto>>(candle);

            return Ok(candleDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<CandleDto>> GetCandleAsync(int id, CancellationToken ct)
        {
            var result = await _unit.Candle.GetCandleAsync(id, ct);

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
