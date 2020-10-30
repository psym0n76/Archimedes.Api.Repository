using System;
using System.Collections.Generic;
using System.Linq;
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
    public class PriceController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<PriceController> _logger;
        private readonly IMapper _mapper;

        public PriceController(IUnitOfWork unit, ILogger<PriceController> logger, IMapper mapper)
        {
            _unit = unit;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PriceDto>>> GetPrices(CancellationToken ct)
        {
            try
            {
                var prices = await _unit.Price.GetPricesAsync(1, 100, ct);

                if (prices != null)
                {
                    return Ok(MapPrices(prices));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError("Price not found");
            return NotFound();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PriceDto>> GetPriceAsync(int id, CancellationToken ct)
        {
            try
            {
                var price = await _unit.Price.GetPriceAsync(id, ct);

                if (price != null)
                {
                    return Ok(MapPrice(price));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError($"Price not found for Id: {id}");
            return NotFound();
        }

        //GET: api/v1/price/bymarket?market=gbpusd
        [HttpGet("bymarket", Name = nameof(GetMarketPricesAsync))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PriceDto>>> GetMarketPricesAsync(string market, CancellationToken ct)
        {
            try
            {
                var marketPrices = await _unit.Price.GetMarketPrices(market, ct);

                if (marketPrices != null)
                {
                    return Ok(MapPrices(marketPrices));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError($"Price not found for market: {market}");
            return NotFound();
        }

        //GET: api/v1/price/bylastupdated?market=gbpusd&granularity=15
        [HttpGet("bylastupdated", Name = nameof(GetLastUpdated))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DateTime>> GetLastUpdated(string market, string granularity,
            CancellationToken ct)
        {

            try
            {
                _logger.LogInformation(
                    $"Request: Get Last Updated Price for Market: {market} and Granularity: {granularity}");

                var lastUpdated = await _unit.Price.GetLastUpdated(market, granularity, ct);

                if (lastUpdated != null)
                {
                    return Ok(lastUpdated);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError($"Price not found for market: {market}.");
            return NotFound();
        }

        //GET: api/v1/price/bymarket_bygranularity_fromdate_todate?market=gbpusd&granularity=15&fromDate=2020-01-01T05:00:00&toDate=2020-01-01T05:00:00
        [HttpGet("bymarket_bygranularity_fromdate_todate", Name = nameof(GetMarketGranularityPricesDate))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PriceDto>>> GetMarketGranularityPricesDate(string market, string granularity,
            string fromDate, string toDate,
            CancellationToken ct)
        {

            try
            {
                _logger.LogInformation(
                    $"Request: Get all Prices for Market: {market} Granularity: {granularity} FromDate: {fromDate} ToDate: {toDate}");

                if (!DateTimeOffset.TryParse(fromDate, out var fromDateOffset))
                {
                    return BadRequest($"Incorrect FromDate format: {fromDate}");
                }

                if (!DateTimeOffset.TryParse(toDate, out var toDateOffset))
                {
                    return BadRequest($"Incorrect ToDate format: {toDate}");
                }

                var prices =
                    await _unit.Price.GetMarketGranularityPricesDate(market, granularity, fromDateOffset, toDateOffset, ct);

                if (prices != null)
                {
                    return Ok(MapPrices(prices));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError(
                $"Price not found. {nameof(market)}: {market} {nameof(granularity)}: {granularity} {nameof(fromDate)}: {fromDate} {nameof(toDate)}: {toDate}");
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("bymarket_distinct", Name = nameof(GetMarketDistinctAsync))]
        public async Task<ActionResult<IEnumerable<PriceDto>>> GetMarketDistinctAsync(CancellationToken ct)
        {
            try
            {
                var prices = await _unit.Price.GetPricesAsync(1, 100000, ct);

                if (prices != null)
                {
                    var distinctGranularity = prices.Select(a => a.Market).Distinct();

                    // create a priceDto collection
                    var priceCollection = distinctGranularity.Select(gran => new PriceDto() {Market = gran}).ToList();

                    return Ok(priceCollection);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError("Prices not found");
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("hour", Name = nameof(DeletePricesAsync))]
        public async Task<ActionResult<IEnumerable<PriceDto>>> DeletePricesAsync(CancellationToken ct)
        {
            try
            {
                await _unit.Price.RemovePricesOlderThanOneHour(ct);
                _logger.LogError($"DELETED Price from Table");
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
        public async Task<ActionResult> PostPrices([FromBody] IList<PriceDto> priceDto, ApiVersion apiVersion,
            CancellationToken ct)
        {
            try
            {
                if (priceDto == null)
                {
                    _logger.LogError($"Received Empty Price update");
                    return null;
                }

                var price = _mapper.Map<IList<Price>>(priceDto);

               // await _unit.Price.RemoveDuplicatePriceEntries(price, ct);
               // _unit.SaveChanges();
                await _unit.Price.AddPricesAsync(price, ct);
                _unit.SaveChanges();

                // re-direct will not work but i wont the 201 response + records added 
                return CreatedAtAction(nameof(GetPrices), new {id = 0, version = apiVersion.ToString()}, price);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }
        }

        private PriceDto MapPrice(Price price)
        {
            return _mapper.Map<PriceDto>(price);
        }

        private IEnumerable<PriceDto> MapPrices(IEnumerable<Price> prices)
        {
            return _mapper.Map<IEnumerable<PriceDto>>(prices);
        }
    }
}
