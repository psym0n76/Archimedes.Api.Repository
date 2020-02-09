using System.Collections.Generic;
using Archimedes.Api.Repository.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

        // GET: api/Price/5
        [HttpGet("{id}", Name = "GetPrice")]
        public IActionResult Get(int id)
        {
            var result = _unit.Price.GetPrice(id);

            if (result == null)
            {
                return NotFound($"Price data not found for Id: {id}");
            }

            var priceDto = _mapper.Map<PriceDto>(result);
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
