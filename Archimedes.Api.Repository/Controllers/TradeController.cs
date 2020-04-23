using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
    public class TradeController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly ILogger<TradeController> _logger;

        public TradeController(IMapper mapper, IUnitOfWork unit, ILogger<TradeController> logger)
        {
            _mapper = mapper;
            _unit = unit;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<TradeDto>>> GetTrades(CancellationToken ct)
        {
            var trade = await _unit.Trade.GetTradesAsync(1, 100, ct);

            if (trade == null)
            {
                _logger.LogError("Trade data not found.");
                return NotFound();
            }

            var tradeDto = _mapper.Map<IEnumerable<TradeDto>>(trade);

            _logger.LogInformation($"Returned {tradeDto.Count()} Trade records");
            return Ok(tradeDto);
        }

        // GET: api/Trade/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<TradeDto>> GetTrade(int id, CancellationToken ct)
        {
            var trade = await _unit.Trade.GetTradeAsync(id, ct);

            if (trade == null)
            {
                _logger.LogError($"Trade data not found for Id: {id}");
                return NotFound();
            }

            var tradeDto = _mapper.Map<TradeDto>(trade);

            _logger.LogInformation("Returned 1 Trade record");
            return Ok(tradeDto);
        }

        // POST: api/Trade
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult PostTrades([FromBody] IEnumerable<TradeDto> tradeDto, CancellationToken ct)
        {
            var trade = _mapper.Map<List<Trade>>(tradeDto);

            _unit.Trade.AddTradesAsync(trade, ct);
            _unit.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrades), new {id = 2 });
        }
    }
}