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
            var candles = await _unit.Candle.GetCandlesAsync(1, 100, ct);

            if (candles != null)
            {
                return Ok(MapCandles(candles));
            }

            _logger.LogError("Candles not found");
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Candle>> GetCandleAsync(int id, CancellationToken ct)
        {
            var candle = await _unit.Candle.GetCandleAsync(id, ct);

            if (candle != null)
            {
                return Ok(MapCandle(candle));
            }

            _logger.LogError($"Candle not found for Id: {id}");
            return NotFound();
        }

        //GET: api/v1/candle/bymarket?market=gbpusd
        [HttpGet("bymarket", Name = nameof(GetMarketCandlesAsync))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CandleDto>>> GetMarketCandlesAsync(string market, CancellationToken ct)
        {
            var marketCandles = await _unit.Candle.GetMarketCandles(market, ct);

            if (marketCandles != null)
            {
                return Ok(MapCandles(marketCandles));
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
            _logger.LogInformation(
                $"Request: Get Last Updated Price for Market: {market} and Granularity: {granularity}");

            var lastUpdated = await _unit.Candle.GetLastCandleUpdated(market, granularity, ct);

            if (lastUpdated != null)
            {
                return Ok(lastUpdated);
            }

            _logger.LogError($"Candle not found for market: {market}.");
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
            _logger.LogInformation(
                $"Request: Get all Candles for Market: {market} Granularity: {granularity} FromDate: {fromDate} ToDate: {toDate}");

            if (!DateTimeOffset.TryParse(fromDate, out var fromDateOffset))
            {
                return BadRequest($"Incorrect FromDate format: {fromDate}");
            }

            if (!DateTimeOffset.TryParse(toDate, out var toDateOffset))
            {
                return BadRequest($"Incorrect ToDate format: {toDate}");
            }

            var candles =
                await _unit.Candle.GetMarketGranularityCandlesDate(market, granularity, fromDateOffset, toDateOffset,
                    ct);

            if (candles != null)
            {
                return Ok(MapCandles(candles));
            }

            _logger.LogError(
                $"Candle not found. {nameof(market)}: {market} {nameof(granularity)}: {granularity} {nameof(fromDate)}: {fromDate} {nameof(toDate)}: {toDate}");
            return NotFound();
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostCandles([FromBody] IList<CandleDto> candleDto, ApiVersion apiVersion,
            CancellationToken ct)
        {
            var marketId = candleDto.Select(a => a.MarketId).FirstOrDefault();
            var maxDate = candleDto.Max(a => a.Timestamp);

            foreach (var p in candleDto)
            {
                _logger.LogInformation($"Received Candle update: \n {p}");
            }

            var candle = _mapper.Map<List<Candle>>(candleDto);

            try
            {
                await _unit.Candle.RemoveDuplicateCandleEntries(candle, ct);
                _unit.SaveChanges();
                await _unit.Candle.AddCandlesAsync(candle, ct);
                _unit.SaveChanges();
                await _unit.Market.UpdateMarketMaxDateAsync(marketId, maxDate, ct);
                _unit.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }

            // re-direct will not work but i wont the 201 response + records added 
            return CreatedAtAction(nameof(GetCandles), new {id = 0, version = apiVersion.ToString()}, candle);
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
