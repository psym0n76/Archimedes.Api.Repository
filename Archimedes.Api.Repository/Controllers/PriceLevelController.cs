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
    [Route("api/v{version:apiVersion}/price-level")]
    [ApiController]
    [Produces("application/json")]
    public class PriceLevelController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<PriceLevelController> _logger;
        private readonly IMapper _mapper;
        private readonly BatchLog _batchLog = new();
        private string _logId;

        public PriceLevelController(IUnitOfWork unit, ILogger<PriceLevelController> logger, IMapper mapper)
        {
            _unit = unit;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PriceLevel>>> GetPriceLevels(CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();

                _batchLog.Update(_logId, $"GET {nameof(GetPriceLevels)}");
                
                var priceLevels = await _unit.PriceLevel.GetPriceLevelsAsync(1, 100000, ct);

                if (priceLevels != null)
                {
                    _logger.LogInformation(
                        _batchLog.Print(_logId, $"Returned {priceLevels.Count()} PriceLevel records"));
                    return Ok(priceLevels.OrderBy(a => a.TimeStamp));
                }

                _logger.LogError(_batchLog.Print(_logId, "PriceLevels not found."));
                return NotFound("Not Found");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound("Operation Cancelled");
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(PriceLevelController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(PriceLevelController)}", e));
            }
        }

        [HttpGet("byMarket_byFromdate", Name = nameof(GetPriceLevelsByMarketFromDate))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<PriceLevel>>> GetPriceLevelsByMarketFromDate(string market,
            DateTime fromDate, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"GET GetPriceLevelsByMarketFromDate {market} {fromDate}");
                
                var priceLevels = await _unit.PriceLevel.GetPriceLevelsByMarketByDateAsync(market, fromDate, ct);

                if (priceLevels != null)
                {
                    _logger.LogInformation(
                        _batchLog.Print(_logId, $"Returned {priceLevels.Count()} PriceLevel records"));
                    return Ok(priceLevels.OrderBy(a => a.TimeStamp));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(PriceLevelController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(PriceLevelController)}", e));
            }

            _logger.LogError(_batchLog.Print(_logId, "PriceLevels not found."));

            return NotFound("Not Found");
        }

        [HttpGet("byMarket_byGranularity_byFromdate", Name = nameof(GetPriceLevelsByMarketByGranularityFromDateActive))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PriceLevel>>> GetPriceLevelsByMarketByGranularityFromDateActive(
            string market, string granularity, DateTime fromDate, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId,$"GET GetPriceLevelsByMarketByGranularityFromDateActive {market} {granularity} {fromDate}");
                
                var priceLevels =
                    await _unit.PriceLevel.GetPriceLevelsByMarketByGranularityByDateActiveAsync(market, granularity,
                        fromDate, ct);

                if (priceLevels != null)
                {
                    _logger.LogInformation(
                        _batchLog.Print(_logId, $"Returned {priceLevels.Count()} PriceLevel records"));
                    return Ok(priceLevels.OrderBy(a => a.TimeStamp));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(PriceLevelController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(PriceLevelController)}", e));
            }

            _logger.LogError(_batchLog.Print(_logId, "PriceLevels not found."));
            return NotFound("Not Found");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PriceLevel>> GetPriceLevelAsync(int id, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                var priceLevel = await _unit.PriceLevel.GetPriceLevelAsync(id, ct);

                if (priceLevel != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId, $"price-level returned {priceLevel.TimeStamp}"));
                    return Ok(priceLevel);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound("Operation Cancelled");
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(PriceLevelController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(PriceLevelController)}", e));
            }

            _logger.LogWarning(_batchLog.Print(_logId, $"price-level not found {id}"));
            return NotFound("Not Found");
        }


        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        
        public async Task<ActionResult<int>> PostPriceLevel([FromBody] PriceLevelDto levelDto,
            ApiVersion apiVersion, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                var level = _mapper.Map<PriceLevel>(levelDto);

                _batchLog.Update(_logId, $"POST PostPriceLevel ({level.TimeStamp})");

                var levelExists = await _unit.PriceLevel.GetPriceLevelExists(level, ct);

                if (levelExists)
                {
                    var message = $"Duplicate PriceLevel {level.TimeStamp} {level.Market} {level.Granularity}";
                    _logger.LogWarning(_batchLog.Print(_logId, message));
                    return UnprocessableEntity(message);
                }

                await _unit.PriceLevel.AddPriceLevelAsync(level, ct);

                _batchLog.Update(_logId, $"PriceLevel {level.TimeStamp} Id={level.Id}) ADDED");

                _unit.SaveChanges();

                _logger.LogInformation(_batchLog.Print(_logId, $"PriceLevel {level.TimeStamp} Id={level.Id}) ADDED"));

                // this now works :)
                return CreatedAtAction(nameof(PostPriceLevel), new {id = level.Id, version = apiVersion.ToString()},
                    level);

            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return BadRequest($"Operation Cancelled");
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(PriceLevelController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(PriceLevelController)}", e));
            }
        }

        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatePriceLevel([FromBody] PriceLevelDto priceLevelDto, ApiVersion apiVersion,
            CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"PUT UpdatePriceLevel {priceLevelDto.Granularity} {priceLevelDto.Market}{priceLevelDto.TimeStamp}");

                var priceLevel = _mapper.Map<PriceLevel>(priceLevelDto);

                await _unit.PriceLevel.UpdatePriceLevelAsync(priceLevel, ct);

                _unit.SaveChanges();

                _logger.LogInformation(_batchLog.Print(_logId, "SAVED"));

                return Ok();
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return BadRequest($"Operation Cancelled");
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(PriceLevelController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(PriceLevelController)}", e));
            }
        }
    }
}