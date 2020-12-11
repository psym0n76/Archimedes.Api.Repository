using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
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
            try
            {
                var markets = await _unit.Market.GetMarketsAsync(1, 100, ct);

                if (markets != null)
                {
                    return Ok(MapMarkets(markets));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError("Markets not found");
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("bymarket_distinct", Name = nameof(GetMarketsDistinctAsync))]
        public async Task<ActionResult<IEnumerable<string>>> GetMarketsDistinctAsync(CancellationToken ct)
        {
            try
            {
                var markets = await _unit.Market.GetMarketsAsync(1, 1000, ct);

                if (markets != null)
                {
                    var distinctMarkets =  markets.Select(a => a.Name).Distinct();

                    return Ok(distinctMarkets);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError("Markets not found");
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("bygranularity_distinct", Name = nameof(GetGranularityDistinctAsync))]
        public async Task<ActionResult<IEnumerable<string>>> GetGranularityDistinctAsync(CancellationToken ct)
        {
            try
            {
                var markets = await _unit.Market.GetMarketsAsync(1, 1000, ct);

                if (markets != null)
                {
                    var distinctGranularity =  markets.Select(a => a.Granularity).Distinct();

                    return Ok(distinctGranularity);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError("Markets not found");
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<MarketDto>> GetMarketAsync(int id, CancellationToken ct)
        {
            try
            {
                var market = await _unit.Market.GetMarketAsync(id, ct);

                if (market != null)
                {
                    return Ok(MapMarket(market));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError($"Market not found for Id: {id}");
            return NotFound();
        }

        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateMarket([FromBody] MarketDto marketDto, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation($"Received Market UPDATE: {marketDto}");
                var market = _mapper.Map<Market>(marketDto);

                await _unit.Market.UpdateMarket(market,ct);

                _unit.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }
        }

        [HttpPut("market_metrics", Name = nameof(UpdateMarketMetrics))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateMarketMetrics([FromBody] MarketDto marketDto, CancellationToken ct)
        {
            try
            {
                var market = _mapper.Map<Market>(marketDto);

                _logger.LogInformation($"Received Market Update: {marketDto}");

                await _unit.Market.UpdateMarketMetrics(market, ct);

                _unit.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }
        }

        [HttpPut("market_status", Name = nameof(UpdateMarketStatus))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateMarketStatus([FromBody] MarketDto marketDto, CancellationToken ct)
        {
            try
            {
                var market = _mapper.Map<Market>(marketDto);

                _logger.LogInformation($"Received Market Status Update: {marketDto}");

                await _unit.Market.UpdateMarketStatus(market, ct);

                _unit.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostMarkets([FromBody] IList<MarketDto> marketDto, ApiVersion apiVersion,
            CancellationToken ct)
        {
            try
            {
                AddLog(marketDto);

                var market = _mapper.Map<List<Market>>(marketDto);

                await _unit.Market.AddMarketsAsync(market,ct);
                _unit.SaveChanges();

                // re-direct will not work but i wont the 201 response + records added 
                //return CreatedAtAction(nameof(GetStrategyAsync), new {id = 0, version = apiVersion.ToString()}, strategy);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }
        }

        private void AddLog(IList<MarketDto> marketDto)
        {
            var log = new StringBuilder();

            foreach (var p in marketDto)
            {
                log.Append(
                    $"  {p}\n");
            }

            log.Append($"\n ADDED {marketDto.Count} Market(s)");

            _logger.LogInformation($"Received Market ADD:\n\n {nameof(marketDto)}\n  {log}\n");
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