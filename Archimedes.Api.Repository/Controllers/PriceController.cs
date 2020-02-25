using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Archimedes.Api.Repository.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Archimedes.Api.Repository.Controllers
{
    [ApiVersion("1.0")]
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PriceController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly ILogger<PriceController> _logger;
        public PriceController(IMapper mapper, IUnitOfWork unit,ILogger<PriceController> logger)
        {
            _mapper = mapper;
            _unit = unit;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"Request: Get all Prices");
            var price = await _unit.Price.GetPrices(1, 100);

            if (price == null)
            {
                _logger.LogError("Price data not found.");
                return NotFound();
            }

            var priceDto = _mapper.Map<IEnumerable<PriceDto>>(price);

            return Ok(priceDto);
        }

        // GET: api/Price/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation($"Request: Get Prices for Id: {id}");
            var price = await _unit.Price.GetPrice(id);

            if (price == null)
            {
                _logger.LogError($"Price data not found for Id: {id}");
                return NotFound();
            }

            var priceDto = _mapper.Map<PriceDto>(price);

            return Ok(priceDto);
        }

        //GET: api/v1/price/bymarket?market=gbpusd
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("bymarket", Name = "GetMarketPrices")]
        public async Task<IActionResult> Get(string market)
        {
            _logger.LogInformation($"Request: Get all Prices for Market: {market}");

            var price = await _unit.Price.GetPrices(a => a.Market == market);

            //removed check as cannot be tested
            if (price == null)
            {
                _logger.LogError($"Price data not found for market: {market}.");
                return NotFound();
            }

            var priceDto = _mapper.Map<IEnumerable<PriceDto>>(price);

            return Ok(priceDto);
        }

        //GET: api/v1/price/bylastupdated?market=gbpusd&granularity=15
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("bylastupdated", Name = "GetLastUpdated")]
        public async Task<IActionResult> Get(string market,string granularity)
        {
            _logger.LogInformation($"Request: Get Last Updated Price for Market: {market} and Granularity: {granularity}");

            var price = await _unit.Price.GetPrices(a => a.Market == market && a.Granularity == granularity);

            //removed check as cannot be tested
            if (price == null)
            {
                _logger.LogError($"Price data not found for market: {market}.");
                return NotFound();
            }

            var priceArray = price as Price[] ?? price.ToArray();
            return Ok(priceArray.Any() ? priceArray.Max(a => a.Timestamp) : DateTimeOffset.MinValue);
        }

        //GET: api/v1/price/bymarket_bygranularity_fromdate_todate?market=gbpusd&granularity=15&fromDate=2020-01-01T05:00:00&toDate=2020-01-01T05:00:00
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("bymarket_bygranularity_fromdate_todate", Name = "GetMarketGranularityPricesDate")]
        public async Task<IActionResult> Get(string market, string granularity,string fromDate, string toDate)
        {
            _logger.LogInformation($"Request: Get all Prices for Market: {market} Granularity: {granularity} FromDate: {fromDate} ToDate: {toDate}");

            if (!DateTimeOffset.TryParse(fromDate, out var fromDateOffset))
            {
                return BadRequest($"Incorrect FromDate format: {fromDate}");
            }

            if (!DateTimeOffset.TryParse(toDate, out var toDateOffset))
            {
                return BadRequest($"Incorrect ToDate format: {toDate}");
            }

            var prices = await _unit.Price.GetPrices(a =>
                a.Market == market && a.Timestamp > fromDateOffset && a.Timestamp <= toDateOffset &&
                a.Granularity == granularity);

            // unable to test due to mocking problems returning a null
            if (prices == null)
            {
                _logger.LogError($"Price data not found. Market: {market} Granularity: {granularity} FromDate: {fromDate} ToDate: {toDate}");
                return NotFound();
            }

            var priceDto = _mapper.Map<IEnumerable<PriceDto>>(prices);

            _logger.LogInformation($"Returned {priceDto.Count()} trade records");
            return Ok(priceDto);
        }

        // POST: api/Price
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] IEnumerable<PriceDto> priceDto)
        {
            _logger.LogInformation($"Request: Post Prices: {priceDto.Count()}");

            var price = _mapper.Map<IEnumerable<Price>>(priceDto).ToList();

            RemoveDuplicatePriceEntries(price);

            await _unit.Price.AddPrices(price);
            var records = await _unit.Complete();

            _logger.LogInformation($"Added {records} price records");
            return Ok();
        }

        private void RemoveDuplicatePriceEntries(IList<Price> price)
        {
            var granularity = price.Select(a => a.Granularity).SingleOrDefault();
            var market = price.Select(a => a.Market).SingleOrDefault();

            var historicPrices = _unit.Price.GetPrices(a => a.Granularity == granularity && a.Market == market).Result.ToList();

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
