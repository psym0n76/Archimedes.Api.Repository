using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
    public class TradeController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<TradeController> _logger;

        public TradeController(IUnitOfWork unit, ILogger<TradeController> logger)
        {
            _unit = unit;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Trade>>> GetTrades(CancellationToken ct)
        {
            var trade = await _unit.Trade.GetTradesAsync(1, 100, ct);

            if (trade == null)
            {
                _logger.LogError("Trade data not found.");
                return NotFound();
            }

            //var tradeDto = _mapper.Map<IEnumerable<TradeDto>>(trade);

            _logger.LogInformation($"Returned {trade.Count()} Trade records");
            return Ok(trade);
        }

        // GET: api/Trade/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Trade>> GetTrade(int id, CancellationToken ct)
        {
            var trade = await _unit.Trade.GetTradeAsync(id, ct);

            if (trade == null)
            {
                _logger.LogError($"Trade data not found for Id: {id}");
                return NotFound();
            }

            //var tradeDto = _mapper.Map<Trade>(trade);

            _logger.LogInformation("Returned 1 Trade record");
            return Ok(trade);
        }

        // POST: api/Trade
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult PostTrades([FromBody] IEnumerable<Trade> trade, CancellationToken ct)
        {
           //var trade = _mapper.Map<List<Trade>>(tradeDto);

            _unit.Trade.AddTradesAsync(trade, ct);
            _unit.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrades), new {id = 2 });
        }
    }
}