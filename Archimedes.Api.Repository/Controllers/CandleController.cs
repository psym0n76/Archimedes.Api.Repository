using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
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
    public class CandleController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<CandleController> _logger;
        private readonly IMapper _mapper;

        public CandleController(IUnitOfWork unit, ILogger<CandleController> logger, IMapper mapper)
        {
            _unit = unit;
            _logger = logger;
            _mapper = mapper;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CandleDto>>> GetCandles(CancellationToken ct)
        {
            try
            {
                var candles = await _unit.Candle.GetCandlesAsync(1, 1000000, ct);

                if (candles != null)
                {
                    return Ok(MapCandles(candles.OrderBy(order => order.TimeStamp)));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError("Candles not found");
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Candle>> GetCandleAsync(int id, CancellationToken ct)
        {
            try
            {
                var candle = await _unit.Candle.GetCandleAsync(id, ct);

                if (candle != null)
                {
                    return Ok(MapCandle(candle));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError($"Candle not found for Id: {id}");
            return NotFound();
        }

        //GET: api/v1/candle/bymarket?market=gbpusd
        [HttpGet("bymarket", Name = nameof(GetMarketCandlesAsync))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CandleDto>>> GetMarketCandlesAsync(string market,
            CancellationToken ct)
        {
            try
            {
                var marketCandles = await _unit.Candle.GetMarketCandles(market, ct);

                if (marketCandles != null)
                {
                    return Ok(MapCandles(marketCandles.OrderBy(a=>a.TimeStamp)));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError($"Candle not found for market: {market}");
            return NotFound();
        }

        //GET: api/v1/candle/bylastupdated?market=gbpusd&granularity=15
        [HttpGet("bylastupdated", Name = nameof(GetLastCandleUpdated))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DateTime>> GetLastCandleUpdated(string market, string granularity,
            CancellationToken ct)
        {
            try
            {
                var lastUpdated = await _unit.Candle.GetLastCandleUpdated(market, granularity, ct);

                if (lastUpdated != null)
                {
                    return Ok(lastUpdated);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            return NotFound();
        }

        //GET: api/v1/candle/bymarket_bygranularity_fromdate_todate?market=gbpusd&granularity=15&fromDate=2020-01-01T05:00:00&toDate=2020-01-01T05:00:00
        [HttpGet("bymarket_bygranularity_fromdate_todate", Name = nameof(GetMarketGranularityCandlesDate))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CandleDto>> GetMarketGranularityCandlesDate(string market, string granularity,
            string fromDate, string toDate,
            CancellationToken ct)
        {
            try
            {
                if (!DateTimeOffset.TryParse(fromDate, out var fromDateOffset))
                {
                    return BadRequest($"Incorrect FromDate format: {fromDate}");
                }

                if (!DateTimeOffset.TryParse(toDate, out var toDateOffset))
                {
                    return BadRequest($"Incorrect ToDate format: {toDate}");
                }

                var candles =
                    await _unit.Candle.GetMarketGranularityCandlesDate(market, granularity, fromDateOffset,
                        toDateOffset,
                        ct);

                if (candles != null)
                {
                    return Ok(MapCandles(candles.OrderBy(a=>a.TimeStamp)));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            _logger.LogError(
                $"Candle not found. {nameof(market)}: {market} {nameof(granularity)}: {granularity} {nameof(fromDate)}: {fromDate} {nameof(toDate)}: {toDate}");
            return NotFound();
        }

        //GET: api/v1/candle/bymarket_bygranularity?market=gbpusd&granularity=15
        [HttpGet("bymarket_bygranularity", Name = nameof(GetMarketGranularityCandles))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CandleDto>> GetMarketGranularityCandles(string market, string granularity,
            CancellationToken ct)
        {
            try
            {
                var candles =
                    await _unit.Candle.GetMarketGranularityCandles(market, granularity, ct);

                if (candles != null)
                {
                    return Ok(MapCandles(candles.OrderBy(order => order.TimeStamp)));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }


            _logger.LogError(
                $"Candle not found. {nameof(market)}: {market} {nameof(granularity)}: {granularity}");
            return NotFound();
        }

        //GET: api/v1/candle/candle_count?market=GBP/USD&granularity=15Min
        [HttpGet("candle_metrics", Name = nameof(GetCandleMetrics))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CandleMetricsDto>> GetCandleMetrics(string market, string granularity,
            CancellationToken ct)
        {
            try
            {
                var result = new CandleMetricsDto()
                {
                    MinDate = await _unit.Candle.GetFirstCandleUpdated(market, granularity, ct),
                    Quantity = await _unit.Candle.GetCandleCount(market, granularity, ct),
                    MaxDate = await _unit.Candle.GetLastCandleUpdated(market, granularity, ct)
                };

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostCandles([FromBody] IList<CandleDto> candleDto, ApiVersion apiVersion,
            CancellationToken ct)
        {
            try
            {
                AddLog(candleDto);

                var candle = _mapper.Map<List<Candle>>(candleDto);

                await _unit.Candle.RemoveDuplicateCandleEntries(candle, ct);
                _unit.SaveChanges(); // not sure this is required
                await _unit.Candle.AddCandlesAsync(candle, ct);
                _unit.SaveChanges();

                // re-direct will not work but i wont the 201 response + records added 
                return CreatedAtAction(nameof(GetCandles), new {id = 0, version = apiVersion.ToString()}, candle);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }
        }

        private void AddLog(IList<CandleDto> candleDto)
        {
            var log = new StringBuilder();

            foreach (var p in candleDto)
            {
                log.Append(
                    $"  {nameof(p.TimeStamp)}: {p.TimeStamp} {nameof(p.Market)}: {p.Market}  {nameof(p.Granularity)}: {p.Granularity} {nameof(p.BidOpen)}: {p.BidOpen} {nameof(p.BidHigh)}: {p.BidHigh} {nameof(p.BidLow)}: {p.BidLow} {nameof(p.BidClose)}: {p.BidClose}\n");
            }

            log.Append($"\n ADDED {candleDto.Count} Candle(s)");

            _logger.LogInformation($"Received Candle ADD:\n\n {nameof(candleDto)}\n  {log}\n");
        }

        private CandleDto MapCandle(Candle candle)
        {
            return _mapper.Map<CandleDto>(candle);
        }

        private IEnumerable<CandleDto> MapCandles(IEnumerable<Candle> candles)
        {
            return _mapper.Map<IEnumerable<CandleDto>>(candles);
        }
    }
}
