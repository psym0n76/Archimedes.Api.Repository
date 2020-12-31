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
        public async Task<ActionResult<List<PriceLevel>>> GetPriceLevels(CancellationToken ct)
        {
            try
            {
                _logId =  _batchLog.Start();
                
                var priceLevels = await _unit.PriceLevel.GetPriceLevelsAsync(1, 100000, ct);

                if (priceLevels != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId, $"Returned {priceLevels.Count()} PriceLevel records"));
                    return Ok(priceLevels.OrderBy(a=>a.TimeStamp));
                }
                
                _logger.LogError(_batchLog.Print(_logId,"PriceLevels not found."));
                
                return NotFound();

            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error {e.Message} {e.StackTrace}"));
                return BadRequest();
            }
        }

        [HttpGet("byMarket_byFromdate", Name = nameof(GetPriceLevelsByMarketFromDate))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<PriceLevel>>> GetPriceLevelsByMarketFromDate(string market, DateTime fromDate, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                var priceLevels = await _unit.PriceLevel.GetPriceLevelsByMarketByDateAsync(market, fromDate,ct);

                if (priceLevels != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId, $"Returned {priceLevels.Count()} PriceLevel records"));
                    return Ok(priceLevels.OrderBy(a=>a.TimeStamp));
                }
            }
            catch (Exception e)
            {
                
                _logger.LogError(_batchLog.Print(_logId, $"Error {e.Message} {e.StackTrace}"));
                return BadRequest();
            }

            _logger.LogError(_batchLog.Print(_logId,"PriceLevels not found."));
            
            return NotFound();
        }

        [HttpGet("byMarket_byGranularity_byFromdate", Name = nameof(GetPriceLevelsByMarketByGranularityFromDateActive))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<PriceLevel>>> GetPriceLevelsByMarketByGranularityFromDateActive(string market,string granularity, DateTime fromDate, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                var priceLevels = await _unit.PriceLevel.GetPriceLevelsByMarketByGranularityByDateActiveAsync(market, granularity, fromDate, ct);

                if (priceLevels != null)
                {
                    _logger.LogInformation( _batchLog.Print(_logId,$"Returned {priceLevels.Count()} PriceLevel records"));
                    return Ok(priceLevels.OrderBy(a=>a.TimeStamp));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId,$"Error {e.Message} {e.StackTrace}"));
                return BadRequest();
            }

            _logger.LogError(_batchLog.Print(_logId,"PriceLevels not found."));
            return NotFound();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PriceLevel>> GetPriceLevelAsync(int id, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                var priceLevel = await _unit.PriceLevel.GetPriceLevelAsync(id, ct);

                if (priceLevel != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId,$"price-level returned {priceLevel.TimeStamp}"));
                    return Ok(priceLevel);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId,$"Error {e.Message} {e.StackTrace}"));
                return BadRequest();
            }

            _logger.LogError(_batchLog.Print(_logId,$"price-level not found {id}"));
            return NotFound();
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostPriceLevels([FromBody] IList<PriceLevelDto> priceLevelDto, ApiVersion apiVersion, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                var priceLevels = _mapper.Map<List<PriceLevel>>(priceLevelDto);

                _batchLog.Update(_logId,$"Processing price-levels ({priceLevels.Count})");
                
                var updatedPriceLevels =  await _unit.PriceLevel.RemoveDuplicatePriceLevelEntries(priceLevels, ct);

                _batchLog.Update(_logId, $"Processing price-levels - identified  ({priceLevels.Count - updatedPriceLevels.Count}) duplicate(s)");

                await _unit.PriceLevel.AddPriceLevelsAsync(updatedPriceLevels, ct);

                _batchLog.Update(_logId, $"Processing price-levels ({updatedPriceLevels.Count}) POSTED");
                
                _unit.SaveChanges();

                _logger.LogInformation(_batchLog.Print(_logId,"SAVED"));

                // leave the re-route in as an example how to do it - cannot have name GetTradesAsync
               //return CreatedAtAction(nameof(GetPriceLevelAsync), new {id = 0, version = apiVersion.ToString()}, priceLevels);
               return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId,$"Error {e.Message} {e.StackTrace}"));
                return BadRequest();
            }
        }

        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatePriceLevel([FromBody] PriceLevelDto priceLevelDto, ApiVersion apiVersion, CancellationToken ct)
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
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId,$"Error {e.Message} {e.StackTrace}"));
                return BadRequest();
            }
        }
    }
}