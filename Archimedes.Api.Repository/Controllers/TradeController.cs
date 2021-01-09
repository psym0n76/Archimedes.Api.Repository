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
    [Produces("application/json")]
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Trade>>> GetTrades(CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"GET GetTrades [Max 100 Trad]" );
                var trades = await _unit.Trade.GetTradesAsync(1, 100, ct);

                if (trades != null)
                {
                    _logger.LogInformation($"Returned {trades.Count()} Trade(s)");
                    return Ok(trades);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from TradeController", e));
                return BadRequest();
            }

            _logger.LogWarning(_batchLog.Print(_logId, $"Trade not found"));
            return NotFound();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Trade>> GetTradeAsync(int id, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"GET GetTradeAsync for {id}");
                
                var trade = await _unit.Trade.GetTradeAsync(id, ct);

                if (trade != null)
                {
                    _logger.LogInformation($"Returned {trade.Strategy} {trade.BuySell} {trade.TimeStamp} Trade");
                    return Ok(trade);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from TradeController", e));
                return BadRequest();
            }

            _logger.LogWarning(_batchLog.Print(_logId, $"Trade not found"));
            return NotFound();
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult PostTrades([FromBody] IList<Trade> trades, ApiVersion apiVersion, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"POST PostTrades {trades.Count} Trade(s)");
                
                _unit.Trade.AddTradesAsync(trades, ct);
                _unit.SaveChanges();

                _logger.LogInformation($"SAVED");

                // leave the re-route in as an example how to do it - cannot have name GetTradesAsync
                return CreatedAtAction(nameof(GetTrades), new {id = 0, version = apiVersion.ToString()}, trades);
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from TradeController", e));
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
                _batchLog.Update(_logId, $"PUT UpdateTrade {tradeDto.Strategy} {tradeDto.BuySell} {tradeDto.Timestamp}");

                var trade = _mapper.Map<Trade>(tradeDto);

                await _unit.Trade.UpdateTradeAsync(trade, ct);

                _unit.SaveChanges();

                _logger.LogInformation(_batchLog.Print(_logId, "SAVED"));

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from TradeController", e));
                return BadRequest();
            }
        }
    }
}