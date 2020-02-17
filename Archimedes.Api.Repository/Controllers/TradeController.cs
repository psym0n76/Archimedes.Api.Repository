using System.Collections.Generic;
using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Archimedes.Api.Repository.Controllers
{
    [ApiVersion("1.0")]
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TradeController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly ILogger<TradeController> _logger;

        public TradeController(IMapper mapper, IUnitOfWork unit,ILogger<TradeController> logger)
        {
            _mapper = mapper;
            _unit = unit;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet()]
        public IActionResult Get()
        {
            var trade = _unit.Trade.GetTrades(1, 100);

            if (trade == null)
            {
                _logger.LogError("Trade data not found.");
                return NotFound();
            }

            var tradeDto = _mapper.Map<IEnumerable<TradeDto>>(trade);

            _logger.LogInformation($"Returned {tradeDto} Trade records");
            return Ok(tradeDto);
        }

        // GET: api/Trade/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var trade = _unit.Trade.GetTrade(id);

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
        public IActionResult Post([FromBody] IEnumerable<TradeDto> tradeDto)
        {
            var trade = _mapper.Map<IEnumerable<Trade>>(tradeDto);
            _unit.Trade.AddTrades(trade);

            var records = _unit.Complete();

            _logger.LogInformation($"Added {records} Trade records");
            return Ok();
        }
    }
}