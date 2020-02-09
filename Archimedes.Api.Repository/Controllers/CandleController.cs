using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Archimedes.Api.Repository.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CandleController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        public CandleController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        // GET: api/Candle
        [HttpGet(Name = "GetCandles")]
        public IActionResult Get()
        {
            var result = _unit.Candle.GetCandles(1, 100);

            if (result == null)
            {
                return NotFound("Candle data not found.");
            }

            var json = JsonConvert.SerializeObject(result);

            return Ok(json);
        }

        // GET: api/Candle/5
        [HttpGet("{id}", Name = "GetCandle")]
        public IActionResult Get(int id)
        {
            var result = _unit.Candle.GetCandle(id);

            if (result == null)
            {
                return NotFound($"Candle data not found for Id: {id}");
            }

            var json = JsonConvert.SerializeObject(result);

            return Ok(json);
        }
    }
}
