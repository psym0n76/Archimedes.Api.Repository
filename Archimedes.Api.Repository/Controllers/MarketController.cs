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
    public class MarketController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<MarketController> _logger;

        public MarketController(IUnitOfWork unit, ILogger<MarketController> logger)
        {
            _unit = unit;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Market>>> GetMarketsAsync(CancellationToken ct)
        {
            var markets = await _unit.Market.GetMarketsAsync(1, 100, ct);

            if (markets != null)
            {
                return Ok(markets);
            }

            _logger.LogError("Markets not found");
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Market>> GetMarketAsync(int id, CancellationToken ct)
        {
            var market = await _unit.Market.GetMarketAsync(id, ct);

            if (market != null)
            {
                return Ok(market);
            }

            _logger.LogError($"Candle not found for Id: {id}");
            return NotFound();
        }
    }
}
