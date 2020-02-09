using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Archimedes.Api.Repository.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PriceController : ControllerBase
    {
        private readonly IUnitOfWork _unit;

        // GET: api/Price
        public PriceController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        [HttpGet(Name = "GetPrices")]
        public IActionResult Get()
        {
            var result = _unit.Price.GetPrices(1, 100);

            if (result == null)
            {
                return NotFound("Price data not found.");
            }

            var json = JsonConvert.SerializeObject(result);

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

            var json = JsonConvert.SerializeObject(result);

            return Ok(json);
        }
    }
}
