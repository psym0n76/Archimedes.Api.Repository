using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
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

        public PriceController(IUnitOfWork unit, ILogger<PriceController> logger)
        {
            _unit = unit;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Price>>> GetPrices(CancellationToken ct)
        {
            var price = await _unit.Price.GetPricesAsync(1, 100, ct);

            if (price != null)
            {
                return Ok(price);
            }

            _logger.LogError("Price data not found.");
            return NotFound();
        }

        // GET: api/Price/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Price>> GetPriceAsync(int id, CancellationToken ct)
        {
            _logger.LogInformation($"Request: Get Prices for Id: {id}");
            var price = await _unit.Price.GetPriceAsync(id, ct);

            if (price != null)
            {
                return Ok(price);
            }

            _logger.LogError($"Price data not found for Id: {id}");
            return NotFound();
        }

        //GET: api/v1/price/bymarket?market=gbpusd
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("bymarket", Name = nameof(GetMarketPricesAsync))]
        public async Task<ActionResult<IEnumerable<Price>>> GetMarketPricesAsync(string market, CancellationToken ct)
        {
            _logger.LogInformation($"Request: Get all Prices for Market: {market}");

            var price = await _unit.Price.GetMarketPrices(market, ct);

            if (price != null)
            {
                return Ok(price);
            }

            _logger.LogError($"Price data not found for market: {market}.");
            return NotFound();
        }

        //GET: api/v1/price/bylastupdated?market=gbpusd&granularity=15
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("bylastupdated", Name = nameof(GetLastUpdated))]
        public async Task<ActionResult<DateTime>> GetLastUpdated(string market, string granularity,
            CancellationToken ct)
        {
            _logger.LogInformation(
                $"Request: Get Last Updated Price for Market: {market} and Granularity: {granularity}");

            var lastUpdated = await _unit.Price.GetLastUpdated(market, granularity, ct);

            if (lastUpdated != null)
            {
                return Ok(lastUpdated);
            }

            _logger.LogError($"Price data not found for market: {market}.");
            return NotFound();
        }

        //GET: api/v1/price/bymarket_bygranularity_fromdate_todate?market=gbpusd&granularity=15&fromDate=2020-01-01T05:00:00&toDate=2020-01-01T05:00:00
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("bymarket_bygranularity_fromdate_todate", Name = nameof(GetMarketGranularityPricesDate))]
        public async Task<ActionResult<Price>> GetMarketGranularityPricesDate(string market, string granularity,
            string fromDate, string toDate,
            CancellationToken ct)
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
                return Ok(prices);
            }

            _logger.LogError(
                $"Price data not found. Market: {market} Granularity: {granularity} FromDate: {fromDate} ToDate: {toDate}");
            return NotFound();
        }


        // POST: api/Price
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostPrices([FromBody] IList<Price> price, ApiVersion apiVersion,
            CancellationToken ct)
        {

            await _unit.Price.RemoveDuplicatePriceEntries(price, ct);
            _unit.SaveChanges();
            await _unit.Price.AddPricesAsync(price, ct);
            _unit.SaveChanges();

            return CreatedAtAction(nameof(GetPrices), new {id = 0, version = apiVersion.ToString()}, price);
        }
    }
}
