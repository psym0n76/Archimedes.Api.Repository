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
    [Route("api/v{version:apiVersion}/price-level")]
    [ApiController]
    public class PriceLevelController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<PriceLevelController> _logger;
        private readonly IMapper _mapper;

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
                var priceLevels = await _unit.PriceLevel.GetPriceLevelsAsync(1, 10000, ct);

                if (priceLevels != null)
                {
                    _logger.LogInformation($"Returned {priceLevels.Count()} PriceLevel records");
                    return Ok(priceLevels.OrderByDescending(a=>a.TimeStamp).Take(1000));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError("PriceLevels not found.");
            return NotFound();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PriceLevel>> GetPriceLevelAsync(int id, CancellationToken ct)
        {
            try
            {
                var priceLevel = await _unit.PriceLevel.GetPriceLevelAsync(id, ct);

                if (priceLevel != null)
                {
                    _logger.LogInformation("Returned 1 PriceLevel record");
                    return Ok(priceLevel);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError($"PriceLevel not found Id: {id}");
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
                var priceLevels = _mapper.Map<List<PriceLevel>>(priceLevelDto);

                AddLog(priceLevels);

                await _unit.PriceLevel.RemoveDuplicatePriceLevelEntries(priceLevels, ct);
                _unit.SaveChanges();
                await _unit.PriceLevel.AddPriceLevelsAsync(priceLevels, ct);
                _unit.SaveChanges();

                // leave the re-route in as an example how to do it - cannot have name GetTradesAsync
               //return CreatedAtAction(nameof(GetPriceLevelAsync), new {id = 0, version = apiVersion.ToString()}, priceLevels);
               return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }
        }

        private void AddLog(IList<PriceLevel> priceLevel)
        {
            var log = new StringBuilder();

            foreach (var p in priceLevel)
            {
                log.Append(
                    $"  {nameof(p.Market)}: {p.Market} {nameof(p.Granularity)}: {p.Granularity}  {nameof(p.Active)}: {p.Active} {nameof(p.TradeType)}: {p.TradeType} {nameof(p.TimeStamp)}: {p.TimeStamp} {nameof(p.Strategy)}: {p.Strategy} {nameof(p.BidPrice)}: {p.BidPrice} {nameof(p.BidPriceRange)}: {p.BidPriceRange} {nameof(p.AskPrice)}: {p.AskPrice} {nameof(p.AskPriceRange)}: {p.AskPriceRange} {nameof(p.LastUpdated)}: {p.LastUpdated}\n");
            }

            log.Append($"\n ADDED {priceLevel.Count} PriceLevel(s)");

            _logger.LogInformation($"Received PriceLevel ADD:\n\n {nameof(priceLevel)}\n  {log}\n");
        }
    }
}