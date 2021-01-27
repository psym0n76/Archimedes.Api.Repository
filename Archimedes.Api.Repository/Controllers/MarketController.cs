using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Archimedes.Library.Logger;
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
        private readonly BatchLog _batchLog = new();
        private string _logId;

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
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
            }

            _logger.LogError("Markets not found");
            return NotFound("Not Found");
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
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
            }

            _logger.LogError("Markets not found");
            return NotFound("Not Found");
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
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
            }

            _logger.LogError("Markets not found");
            return NotFound("Not Found");
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
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
            }

            _logger.LogError($"Market not found for Id: {id}");
            return NotFound("Not Found");
        }

        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateMarket([FromBody] MarketDto marketDto, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"Processing market UPDATE: {marketDto.Name}");

                var market = _mapper.Map<Market>(marketDto);

                await _unit.Market.UpdateMarket(market,ct);

                _batchLog.Update(_logId, $"Processing market UPDATED: {marketDto.Name}");

                _unit.SaveChanges();

                _batchLog.Print(_logId, $"SAVED");

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
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
                _logId = _batchLog.Start($"Processing market-metrics UPDATE: {marketDto.Name} Id: {marketDto.Id} ExtId: {marketDto.ExternalMarketId}");

                var market = _mapper.Map<Market>(marketDto);

                if (!await _unit.Market.UpdateMarketMetrics(market, ct))
                {
                    _logger.LogWarning(_batchLog.Print(_logId, $"Market {market.Id} Not Found"));
                    return NotFound(market);
                }

                _unit.SaveChanges();
                _batchLog.Print(_logId, $"SAVED");

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
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
                _logId = _batchLog.Start($"Processing market-status UPDATE: {marketDto.Name} Id: {marketDto.Id} ExtId: {marketDto.ExternalMarketId}");

                var market = _mapper.Map<Market>(marketDto);

                if (!await _unit.Market.UpdateMarketStatus(market, ct))
                {
                    _logger.LogWarning(_batchLog.Print(_logId, $"Market {market.Id} Mot Found"));
                    return NotFound(market);
                }
                
                _unit.SaveChanges();
                _batchLog.Print(_logId, $"SAVED");
                
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
            }
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostMarkets([FromBody] IList<MarketDto> marketDto, ApiVersion apiVersion,
            CancellationToken ct)
        {
            //todo change to individual updates
            
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"Processing markets ADD ({marketDto.Count})");

                var market = _mapper.Map<List<Market>>(marketDto);

                await _unit.Market.AddMarketsAsync(market,ct);

                _batchLog.Update(_logId, $"Processing markets ADDED ({marketDto.Count})");
                _unit.SaveChanges();

                _batchLog.Print(_logId, $"SAVED");

                // re-direct will not work but i wont the 201 response + records added 
                //return CreatedAtAction(nameof(PostMarkets), new {id = market., version = apiVersion.ToString()}, market);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(MarketController)}", e));
            }
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