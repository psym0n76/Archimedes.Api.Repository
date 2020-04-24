using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly ILogger<CandleController> _logger;

        public CandleController(IUnitOfWork unit, ILogger<CandleController> logger)
        {
            _unit = unit;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Candle>>> GetCandlesAsync(CancellationToken ct)
        {
            var candles = await _unit.Candle.GetCandlesAsync(1, 100, ct);

            if (candles != null)
            {
                return Ok(candles);
            }

            _logger.LogError("Candles not found");
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Candle>> GetCandleAsync(int id, CancellationToken ct)
        {
            var candle = await _unit.Candle.GetCandleAsync(id, ct);

            if (candle != null)
            {
                return Ok(candle);
            }

            _logger.LogError($"Candle not found for Id: {id}");
            return NotFound();
        }
    }
}
