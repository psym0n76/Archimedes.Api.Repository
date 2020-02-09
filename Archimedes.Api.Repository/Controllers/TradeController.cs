using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Archimedes.Api.Repository.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TradeController : ControllerBase
    {
        private readonly IUnitOfWork _unit;

        // GET: api/Trade
        public TradeController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        [HttpGet(Name = "GetTrades")]
        public IActionResult Get()
        {
            var result = _unit.Trade.GetTrades(1, 100);

            if (result == null)
            {
                return NotFound("Trade data not found.");
            }

            var json = JsonConvert.SerializeObject(result);

            return Ok(json);
        }

        // GET: api/Trade/5
        [HttpGet("{id}", Name = "GetTrade")]
        public IActionResult Get(int id)
        {
            var result = _unit.Trade.GetTrade(id);

            if (result == null)
            {
                return NotFound($"Trade data not found for Id: {id}");
            }

            var json = JsonConvert.SerializeObject(result);

            return Ok(json);
        }
    }
}