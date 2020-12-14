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
    public class StrategyController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<StrategyController> _logger;
        private readonly IMapper _mapper;

        public StrategyController(IUnitOfWork unit, ILogger<StrategyController> logger, IMapper mapper)
        {
            _unit = unit;
            _logger = logger;
            _mapper = mapper;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StrategyDto>>> GetStrategiesAsync(CancellationToken ct)
        {
            try
            {
                var strategies = await _unit.Strategy.GetStrategiesAsync(1, 100, ct);

                if (strategies != null)
                {
                    return Ok(MapStrategies(strategies));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError("Strategies not found");
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<StrategyDto>> GetStrategyAsync(int id, CancellationToken ct)
        {
            try
            {
                var strategy = await _unit.Strategy.GetStrategyAsync(id, ct);

                if (strategy != null)
                {
                    return Ok(MapStrategy(strategy));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError($"Strategy not found for Id: {id}");
            return NotFound();
        }

        //GET: api/v1/strategy/bymarket_bygranularity?market=gbpusd&granularity=15
        [HttpGet("bymarket_bygranularity", Name = nameof(GetActiveStrategiesGranularityMarket))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StrategyDto>> GetActiveStrategiesGranularityMarket(string market, string granularity,
            CancellationToken ct)
        {
            try
            {
                var strategies =
                    await _unit.Strategy.GetActiveStrategiesGranularityMarket(market, granularity, ct);

                if (strategies != null)
                {
                    return Ok(MapStrategies(strategies));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }


            _logger.LogError(
                $"Candle not found. {nameof(market)}: {market} {nameof(granularity)}: {granularity}");
            return NotFound();
        }

        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateStrategy([FromBody] StrategyDto strategyDto, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation($"Received Strategy UPDATE: {strategyDto}");

                var strategy = _mapper.Map<Strategy>(strategyDto);

                await _unit.Strategy.UpdateStrategy(strategy, ct);

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
        public async Task<ActionResult> PostStrategies([FromBody] IList<StrategyDto> strategyDto, ApiVersion apiVersion,
            CancellationToken ct)
        {
            try
            {
                AddLog(strategyDto);

                var strategy = _mapper.Map<List<Strategy>>(strategyDto);

                // await _unit.Candle.RemoveDuplicateCandleEntries(strategy, ct);
                // _unit.SaveChanges(); // not sure this is required
                await _unit.Strategy.AddStrategiesAsync(strategy,ct);
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


        private void AddLog(IList<StrategyDto> strategyDto)
        {
            var log = new StringBuilder();

            foreach (var p in strategyDto)
            {
                log.Append(
                    $"  {p}\n");
            }

            log.Append($"\n ADDED {strategyDto.Count} Strategies");

            _logger.LogInformation($"Received Strategy ADD:\n\n {nameof(strategyDto)}\n  {log}\n");
        }














        private StrategyDto MapStrategy(Strategy strategy)
        {
            return _mapper.Map<StrategyDto>(strategy);
        }

        private IEnumerable<StrategyDto> MapStrategies(IEnumerable<Strategy> strategies)
        {
            return _mapper.Map<IEnumerable<StrategyDto>>(strategies);
        }
    }
}