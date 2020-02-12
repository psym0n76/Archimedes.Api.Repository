using System.Collections.Generic;
using System.Threading.Tasks;
using Archimedes.Api.Repository.DTO;
using AutoMapper;using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Archimedes.Api.Repository.Controllers
{
    [Route("api/v1/[controller]")]
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

        [HttpGet(Name = "GetPrices")]
        public IActionResult Get()
        {
            var price = _unit.Price.GetPrices(1, 100);

            if (price == null)
            {
                return NotFound("Pricing data not found.");
            }

            var priceDto = _mapper.Map<IEnumerable<PriceDto>>(price);
            var json = JsonConvert.SerializeObject(priceDto);

            return Ok(json);
        }


        //GET: api/v1/price/bymarket?market=gbpusd
        [HttpGet("bymarket", Name = "GetMarketPrices")]
        public  IActionResult Get(string market)
        {
            var price = _unit.Price.GetPrices(a=>a.Market == market);

            if (price == null)
            {
                return NotFound($"Price data not found for Id: {market}");
            }

            var priceDto = _mapper.Map<IEnumerable<PriceDto>>(price);
            var json = JsonConvert.SerializeObject(priceDto);

            return Ok(json);
        }

        //GET: api/v1/price/bymarket_fromdate_todate?market=gbpusd&fromDate=25&toDate=20
        [HttpGet("bymarket_fromdate_todate", Name = "GetMarketPricesDate")]
        public  IActionResult Get(string market,string fromDate,string toDate)
        {
            var price = _unit.Price.GetPrices(a=>a.Market == market);

            if (price == null)
            {
                return NotFound($"Price data not found for Id: {market}");
            }

            var priceDto = _mapper.Map<IEnumerable<PriceDto>>(price);
            var json = JsonConvert.SerializeObject(priceDto);

            return Ok(json);
        }


        // GET: api/Price/5
        [HttpGet("{id}", Name = "GetPrice")]
        public IActionResult Get(int id)
        {
            var price = _unit.Price.GetPrice(id);

            if (price == null)
            {
                return NotFound($"Price data not found for Id: {id}");
            }

            var priceDto = _mapper.Map<PriceDto>(price);
            var json = JsonConvert.SerializeObject(priceDto);

            return Ok(json);
        }

        // POST: api/Price
        [HttpPost(Name = "PostPrice")]
        public IActionResult Post([FromBody] IEnumerable<PriceDto> value)
        {
            var price = _mapper.Map<IEnumerable<Price>>(value);
            _unit.Price.AddPrices(price);
            var result = _unit.Complete();

            return Ok($"Added {result} Price records");
        }
    }
}
