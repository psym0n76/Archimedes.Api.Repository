using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Archimedes.Library.Message.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Archimedes.Api.Repository.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PriceLevelController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<PriceLevelController> _logger;
        private readonly IMapper _mapper;

        public PriceLevelController(IUnitOfWork unit, ILogger<PriceLevelController> logger, IMapper mapper)
        {
            _unit = unit;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PriceLevel>>> GetPriceLevels(CancellationToken ct)
        {
            try
            {
                var priceLevels = await _unit.PriceLevel.GetPriceLevelsAsync(1, 100, ct);

                if (priceLevels != null)
                {
                    _logger.LogInformation($"Returned {priceLevels.Count()} PriceLevel records");
                    return Ok(priceLevels);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError("PriceLevels not found.");
            return NotFound();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PriceLevel>> GetPriceLevelAsync(int id, CancellationToken ct)
        {
            try
            {
                var priceLevel = await _unit.PriceLevel.GetPriceLevelAsync(id, ct);

                if (priceLevel != null)
                {
                    _logger.LogInformation("Returned 1 PriceLevel record");
                    return Ok(priceLevel);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError($"PriceLevel not found Id: {id}");
            return NotFound();
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult PostPriceLevels([FromBody] IList<PriceLevelDto> priceLevelDto, ApiVersion apiVersion, CancellationToken ct)
        {
            try
            {
                var priceLevels = _mapper.Map<List<PriceLevel>>(priceLevelDto);
                _unit.PriceLevel.AddPriceLevelsAsync(priceLevels, ct);
                _unit.SaveChanges();

                // leave the re-route in as an example how to do it - cannot have name GetTradesAsync
                return CreatedAtAction(nameof(GetPriceLevelAsync), new {id = 0, version = apiVersion.ToString()}, priceLevels);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }
        }
    }
}