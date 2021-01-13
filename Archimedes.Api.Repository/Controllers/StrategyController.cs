using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
    public class StrategyController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<StrategyController> _logger;
        private readonly IMapper _mapper;
        private readonly BatchLog _batchLog = new();
        private string _logId;

        public StrategyController(IUnitOfWork unit, ILogger<StrategyController> logger, IMapper mapper)
        {
            _unit = unit;
            _logger = logger;
            _mapper = mapper;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StrategyDto>>> GetStrategiesAsync(CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, "GET GetStrategiesAsync");
                var strategies = await _unit.Strategy.GetStrategiesAsync(1, 100, ct);

                if (strategies != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId,$"Returned {strategies.Count} Strategies"));
                    return Ok(MapStrategies(strategies));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound();
            }

            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from StrategyController", e));
                return BadRequest();
            }

            _logger.LogWarning(_batchLog.Print(_logId,"Strategies not found"));
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        public async Task<ActionResult<StrategyDto>> GetStrategyAsync(int id, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, "GET GetStrategyAsync");
                var strategy = await _unit.Strategy.GetStrategyAsync(id, ct);

                if (strategy != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId, $"Returned 1 Strategies"));
                    return Ok(MapStrategy(strategy));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from StrategyController", e));
                return BadRequest();
            }

            _logger.LogWarning(_batchLog.Print(_logId, $"Not Found"));
            return NotFound();
        }

        //GET: api/v1/strategy/bymarket_bygranularity?market=gbpusd&granularity=15
        [HttpGet("bymarket_bygranularity", Name = nameof(GetActiveStrategiesGranularityMarket))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<StrategyDto>>> GetActiveStrategiesGranularityMarket(string market, string granularity,
            CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"GET GetActiveStrategiesGranularityMarket {market} {granularity}");
                var strategies =
                    await _unit.Strategy.GetActiveStrategiesGranularityMarket(market, granularity, ct);

                if (strategies.Any())
                {
                    _logger.LogInformation(_batchLog.Print(_logId, $"Returned {strategies.Count} Strategies"));
                    return Ok(MapStrategies(strategies));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from StrategyController", e));
                return BadRequest();
            }

            _logger.LogWarning(_batchLog.Print(_logId,"Strategy not found"));
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
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"PUT UpdateStrategy {strategyDto.Market} {strategyDto.Granularity}");

                var strategy = _mapper.Map<Strategy>(strategyDto);

                await _unit.Strategy.UpdateStrategy(strategy, ct);

                _unit.SaveChanges();
                _logger.LogInformation(_batchLog.Print(_logId, "SAVED"));
                return Ok();
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from StrategyController", e));
                return BadRequest();
            }
        }


        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostStrategies([FromBody] List<StrategyDto> strategyDto, ApiVersion apiVersion,
            CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"POST PostStrategies {strategyDto.Count} Records");

                var strategy = _mapper.Map<List<Strategy>>(strategyDto);

                // await _unit.Candle.RemoveDuplicateCandleEntries(strategy, ct);
                // _unit.SaveChanges(); // not sure this is required
                await _unit.Strategy.AddStrategiesAsync(strategy,ct);
                _unit.SaveChanges();
                _logger.LogInformation(_batchLog.Print(_logId,"SAVED"));

                // re-direct will not work but i wont the 201 response + records added 
                //return CreatedAtAction(nameof(GetStrategyAsync), new {id = 0, version = apiVersion.ToString()}, strategy);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from StrategyController", e));
                return BadRequest();
            }
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