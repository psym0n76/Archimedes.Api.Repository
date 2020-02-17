using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
        public IActionResult Get()
        {
            var price = _unit.Price.GetPrices(1, 100);

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
        public IActionResult Get(int id)
        {
            var price = _unit.Price.GetPrice(id);

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
        public IActionResult Get(string market)
        {
            var price = _unit.Price.GetPrices(a => a.Market == market);

            if (price == null)
            {
                _logger.LogError($"Price data not found for market: {market}.");
                return NotFound();
            }

            var priceDto = _mapper.Map<IEnumerable<PriceDto>>(price);

            return Ok(priceDto);
        }

        //GET: api/v1/price/bymarket_bygranularity_fromdate_todate?market=gbpusd&granularity=15&fromDate=25&toDate=20
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("bymarket_bygranularity_fromdate_todate", Name = "GetMarketGranularityPricesDate")]
        public IActionResult Get(string market, string granularity,string fromDate, string toDate)
        {

            if (DateTimeOffset.TryParse(fromDate, out var fromDateOffset))
            {
                return BadRequest($"Incorrect From date format: {fromDate}");
            }

            if (DateTimeOffset.TryParse(toDate, out var toDateOffset))
            {
                return BadRequest($"Incorrect To date format: {toDate}");
            }

            var prices = _unit.Price.GetPrices(a =>
                a.Market == market && a.Timestamp > fromDateOffset && a.Timestamp <= toDateOffset &&
                a.Granularity == granularity);

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
        public IActionResult Post([FromBody] IEnumerable<PriceDto> priceDto)
        {
            var price = _mapper.Map<IEnumerable<Price>>(priceDto).ToList();

            RemoveDuplicatePriceEntries(price);

            _unit.Price.AddPrices(price);
            var records = _unit.Complete();

            
            _logger.LogInformation($"Added {records} trade records");
            return Ok();
        }

        private void RemoveDuplicatePriceEntries(IList<Price> price)
        {
            var granularity = price.Select(a => a.Granularity).SingleOrDefault();
            var market = price.Select(a => a.Market).SingleOrDefault();

            var historicPrices = _unit.Price.GetPrices(a => a.Granularity == granularity && a.Market == market);

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
