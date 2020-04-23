using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<ActionResult<IEnumerable<Price>>> GetPricesAsync(CancellationToken ct)
        {
            var price = await _unit.Price.GetPricesAsync(1, 100, ct);

            if (price == null)
            {
                _logger.LogError("Price data not found.");
                return NotFound();
            }

            return Ok(price);
        }

        // GET: api/Price/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Price>> GetPriceAsync(int id, CancellationToken ct)
        {
            _logger.LogInformation($"Request: Get Prices for Id: {id}");
            var price = await _unit.Price.GetPriceAsync(id, ct);

            if (price == null)
            {
                _logger.LogError($"Price data not found for Id: {id}");
                return NotFound();
            }

            return Ok(price);
        }

        //GET: api/v1/price/bymarket?market=gbpusd
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("bymarket", Name = "GetMarketPrices")]
        public async Task<ActionResult<IEnumerable<Price>>> GetMarketPricesAsync(string market, CancellationToken ct)
        {
            _logger.LogInformation($"Request: Get all Prices for Market: {market}");

            var price = await _unit.Price.GetPricesAsync(a => a.Market == market, ct);

            //removed check as cannot be tested
            if (price == null)
            {
                _logger.LogError($"Price data not found for market: {market}.");
                return NotFound();
            }

            return Ok(price);
        }

        //GET: api/v1/price/bylastupdated?market=gbpusd&granularity=15
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("bylastupdated", Name = "GetLastUpdated")]
        public async Task<ActionResult<DateTime>> GetLastUpdated(string market, string granularity,
            CancellationToken ct)
        {
            _logger.LogInformation(
                $"Request: Get Last Updated Price for Market: {market} and Granularity: {granularity}");

            var lastUpdated = await _unit.Price.GetLastUpdated(market, granularity, ct);

            if (lastUpdated == null)
            {
                _logger.LogError($"Price data not found for market: {market}.");
                return NotFound();
            }

            return Ok(lastUpdated);
        }

        //GET: api/v1/price/bymarket_bygranularity_fromdate_todate?market=gbpusd&granularity=15&fromDate=2020-01-01T05:00:00&toDate=2020-01-01T05:00:00
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("bymarket_bygranularity_fromdate_todate", Name = "GetMarketGranularityPricesDate")]
        public async Task<ActionResult<Price>> GetMarketGranularityPricesDate(string market, string granularity, string fromDate, string toDate,
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

            var prices = await _unit.Price.GetMarketGranularityPricesDate(market, granularity, fromDateOffset, toDateOffset, ct);

            // unable to test due to mocking problems returning a null
            if (prices == null)
            {
                _logger.LogError(
                    $"Price data not found. Market: {market} Granularity: {granularity} FromDate: {fromDate} ToDate: {toDate}");
                return NotFound();
            }

            return Ok(prices);
        }

        // POST: api/Price
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostPrice([FromBody] IList<Price> price, CancellationToken ct)
        {
            _logger.LogInformation($"Request: PostPrice Prices: {price.Count()}");

            //var price = _mapper.Map<IEnumerable<Price>>(priceDto).ToList();

            RemoveDuplicatePriceEntries(price, ct);

            await _unit.Price.AddPricesAsync(price);
            var records = await _unit.SaveChangesAsync();

            _logger.LogInformation($"Added {records} price records");
            return Ok();
        }

        private void RemoveDuplicatePriceEntries(IList<Price> price, CancellationToken ct)
        {
            var granularity = price.Select(a => a.Granularity).SingleOrDefault();
            var market = price.Select(a => a.Market).SingleOrDefault();

            var historicPrices = _unit.Price.GetPricesAsync(a => a.Granularity == granularity && a.Market == market, ct)
                .Result
                .ToList();

            _logger.LogInformation($"Identified {historicPrices.Count} duplicate price records to delete");

            var result = historicPrices.Join(price, hist => hist.Timestamp, current => current.Timestamp,
                (hist, current) => new Price
                {
                    Id = hist.Id,
                    Market = hist.Market,
                    Granularity = hist.Granularity,
                    AskOpen = hist.AskOpen,
                    AskHigh = hist.AskHigh,
                    AskLow = hist.AskLow,
                    AskClose = hist.AskClose,
                    BidOpen = hist.BidOpen,
                    BidHigh = hist.BidHigh,
                    BidLow = hist.BidLow,
                    BidClose = hist.BidClose,
                    Timestamp = hist.Timestamp
                }).ToList();

            _unit.Price.RemoveRange(result);
        }
    }
}
