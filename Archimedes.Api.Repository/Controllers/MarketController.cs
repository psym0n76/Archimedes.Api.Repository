using System.Collections.Generic;
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
    public class MarketController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<MarketController> _logger;
        private readonly IMapper _mapper;

        public MarketController(IUnitOfWork unit, ILogger<MarketController> logger, IMapper mapper)
        {
            _unit = unit;
            _logger = logger;
            _mapper = mapper;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarketDto>>> GetMarketsAsync(CancellationToken ct)
        {
            var markets = await _unit.Market.GetMarketsAsync(1, 100, ct);

            if (markets != null)
            {
                return Ok(MapMarkets(markets));
            }

            _logger.LogError("Markets not found");
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<MarketDto>> GetMarketAsync(int id, CancellationToken ct)
        {
            var market = await _unit.Market.GetMarketAsync(id, ct);

            if (market != null)
            {
                return Ok(MapMarket(market));
            }

            _logger.LogError($"Market not found for Id: {id}");
            return NotFound();
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateMarketMaxDate([FromBody] MarketDto market, CancellationToken ct)
        {
            // this si not used i think
            var markets = await _unit.Market.GetMarketsAsync(1,1000,ct);

            if (markets != null)
            {
                return Ok(market);
            }

            return NotFound();
        }

        private MarketDto MapMarket(Market market)
        {
            return _mapper.Map<MarketDto>(market);
        }

        private IEnumerable<MarketDto> MapMarkets(IEnumerable<Market> markets)
        {
            return _mapper.Map<IEnumerable<MarketDto>>(markets);
        }
    }
}
