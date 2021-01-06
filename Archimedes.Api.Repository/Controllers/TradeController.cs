using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Archimedes.Library.Logger;
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
    public class TradeController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<TradeController> _logger;
        private readonly BatchLog _batchLog = new();
        private string _logId;
        private readonly IMapper _mapper;

        public TradeController(IUnitOfWork unit, ILogger<TradeController> logger, IMapper mapper)
        {
            _unit = unit;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Trade>>> GetTrades(CancellationToken ct)
        {
            try
            {
                var trades = await _unit.Trade.GetTradesAsync(1, 100, ct);

                if (trades != null)
                {
                    _logger.LogInformation($"Returned {trades.Count()} Trade records");
                    return Ok(trades);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError("Trade not found.");
            return NotFound();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Trade>> GetTradeAsync(int id, CancellationToken ct)
        {
            try
            {
                var trade = await _unit.Trade.GetTradeAsync(id, ct);

                if (trade != null)
                {
                    _logger.LogInformation("Returned 1 Trade record");
                    return Ok(trade);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError($"Trade not found Id: {id}");
            return NotFound();
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult PostTrades([FromBody] IList<Trade> trade, ApiVersion apiVersion, CancellationToken ct)
        {
            try
            {
                _unit.Trade.AddTradesAsync(trade, ct);
                _unit.SaveChanges();

                // leave the re-route in as an example how to do it - cannot have name GetTradesAsync
                return CreatedAtAction(nameof(GetTrades), new {id = 0, version = apiVersion.ToString()}, trade);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }
        }


        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateTrade([FromBody] TradeDto tradeDto, ApiVersion apiVersion, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"Processing trade UPDATE {tradeDto.Timestamp}");

                var trade = _mapper.Map<Trade>(tradeDto);

                await _unit.Trade.UpdateTradeAsync(trade, ct);

                _batchLog.Update(_logId, $"Processing trade UPDATED {tradeDto.Timestamp}");
                _unit.SaveChanges();

                _logger.LogInformation(_batchLog.Print(_logId, "SAVED"));

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error {e.Message} {e.StackTrace}"));
                return BadRequest();
            }
        }
    }
}