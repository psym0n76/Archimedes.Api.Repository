using System;
using System.Collections.Generic;
using System.Net.Mime;
using Archimedes.Api.Repository.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public PriceController(IMapper mapper, IUnitOfWork unit)
        {
            _mapper = mapper;
            _unit = unit;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet()]
        public IActionResult Get()
        {
            var price = _unit.Price.GetPrices(1, 100);

            if (price == null)
            {
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
                return NotFound();
            }

            var priceDto = _mapper.Map<IEnumerable<PriceDto>>(price);

            return Ok(priceDto);
        }

        //GET: api/v1/price/bymarket_fromdate_todate?market=gbpusd&fromDate=25&toDate=20&granularity=15
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("bymarket_fromdate_todate_granularity", Name = "GetMarketPricesDateGranularity")]
        public IActionResult Get(string market, string fromDate, string toDate, string granularity)
        {

            if (DateTimeOffset.TryParse(fromDate, out var fromDateOffset))
            {
                return BadRequest($"Incorrect date format: {fromDate}");
            }

            if (DateTimeOffset.TryParse(toDate, out var toDateOffset))
            {
                return BadRequest($"Incorrect date format: {toDate}");
            }

            var prices = _unit.Price.GetPrices(a =>
                a.Market == market && a.Timestamp > fromDateOffset && a.Timestamp <= toDateOffset &&
                a.Granularity == granularity);

            if (prices == null)
            {
                return NotFound();
            }

            var priceDto = _mapper.Map<IEnumerable<PriceDto>>(prices);

            return Ok(priceDto);
        }

        // POST: api/Price
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] IEnumerable<PriceDto> value)
        {
            var price = _mapper.Map<IEnumerable<Price>>(value);
            _unit.Price.AddPrices(price);
            var result = _unit.Complete();

            return Ok($"Records added: {result}");
        }
    }
}
