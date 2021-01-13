﻿using System;
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

                var priceLevels = await _unit.PriceLevel.GetPriceLevelsAsync(1, 100000, ct);

                if (priceLevels != null)
                {
                    _logger.LogInformation(
                        _batchLog.Print(_logId, $"Returned {priceLevels.Count()} PriceLevel records"));
                    return Ok(priceLevels.OrderBy(a => a.TimeStamp));
                }

                _logger.LogError(_batchLog.Print(_logId, "PriceLevels not found."));

                return NotFound();
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceLevelController", e));
                return BadRequest(e.Message);
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

                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceLevelController", e));
                return BadRequest(e.Message);
            }

            _logger.LogError(_batchLog.Print(_logId, "PriceLevels not found."));

            return NotFound();
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
                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceLevelController", e));
                return BadRequest(e.Message);
            }

            _logger.LogError(_batchLog.Print(_logId, "PriceLevels not found."));
            return NotFound();
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
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceLevelController", e));
                return BadRequest(e.Message);
            }

            _logger.LogWarning(_batchLog.Print(_logId, $"price-level not found {id}"));
            return NotFound();
        }


        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> PostPriceLevel([FromBody] PriceLevelDto priceLevelDto,
            ApiVersion apiVersion, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                var level = _mapper.Map<PriceLevel>(priceLevelDto);

                _batchLog.Update(_logId, $"Processing PriceLevel ({level.TimeStamp})");

                var levelExists = await _unit.PriceLevel.GetPriceLevelExists(level, ct);

                if (levelExists)
                {
                    _logger.LogWarning(_batchLog.Print(_logId,
                        $"Duplicate PriceLevel {priceLevelDto.TimeStamp} {priceLevelDto.Market} {priceLevelDto.Granularity}"));
                    return Ok(
                        $"Duplicate PriceLevel {priceLevelDto.TimeStamp} {priceLevelDto.Market} {priceLevelDto.Granularity}");
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
                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceLevelController", e));
                return BadRequest(e.Message);
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
                _batchLog.Update(_logId, $"Processing price-level UPDATE {priceLevelDto.TimeStamp}");

                var priceLevel = _mapper.Map<PriceLevel>(priceLevelDto);

                await _unit.PriceLevel.UpdatePriceLevelAsync(priceLevel, ct);

                _batchLog.Update(_logId, $"Processing price-level UPDATED {priceLevelDto.TimeStamp}");

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
                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceLevelController", e));
                return BadRequest(e.Message);
            }
        }
    }
}