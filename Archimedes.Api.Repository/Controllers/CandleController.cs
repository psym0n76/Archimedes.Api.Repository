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
    public class CandleController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<CandleController> _logger;
        private readonly IMapper _mapper;
        private readonly BatchLog _batchLog = new BatchLog();
        private string _logId;

        public CandleController(IUnitOfWork unit, ILogger<CandleController> logger, IMapper mapper)
        {
            _unit = unit;
            _logger = logger;
            _mapper = mapper;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CandleDto>>> GetCandles(CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, "GET GetCandles [MAX 1000000 records]");
                var candles = await _unit.Candle.GetCandlesAsync(1, 1000000, ct);

                if (candles != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId,$"Returned {candles.Count} Candle(s)"));
                    return Ok(MapCandles(candles.OrderBy(order => order.TimeStamp)));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound("Operation Cancelled");
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
            }

            _logger.LogWarning(_batchLog.Print(_logId, $"Returned 0 Candle(s)"));
            return NotFound("Not Found");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Candle>> GetCandleAsync(int id, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"GET GetCandleAsync {id} request");
                var candle = await _unit.Candle.GetCandleAsync(id, ct);

                if (candle != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId, $"Returned {candle.Market} {candle.Granularity} {candle.TimeStamp} Candle"));
                    return Ok(MapCandle(candle));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound("Operation Cancelled");
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
            }

            _logger.LogWarning(_batchLog.Print(_logId, $"Returned 0 Candle(s)"));
            return NotFound("Not Found");
        }

        //GET: api/v1/candle/bypage?page=1&size50
        [HttpGet("bypage", Name = nameof(GetCandlesAsync))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CandleDto>>> GetCandlesAsync(int page, int size, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"GET GetCandleAsync Page: {page} Size: {size} request");
                var candles = await _unit.Candle.GetCandlesAsync(page, size,ct);

                if (candles != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId, $"Returned {candles.Count} Candle(s)"));
                    return Ok(MapCandles(candles.OrderBy(a=>a.TimeStamp)));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound("Operation Cancelled");
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
            }

            _logger.LogWarning(_batchLog.Print(_logId, $"Returned 0 Candle(s)"));
            return NotFound("Not Found");
        }

        //GET: api/v1/candle/bymarket?market=gbpusd
        [HttpGet("bymarket", Name = nameof(GetMarketCandlesAsync))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CandleDto>>> GetMarketCandlesAsync(string market,
            CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId,$"GET GetMarketCandlesAsync for {market}");
                var marketCandles = await _unit.Candle.GetMarketCandles(market, ct);

                if (marketCandles != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId,$"Returned {marketCandles.Count} Candles(s)"));
                    return Ok(MapCandles(marketCandles.OrderBy(a=>a.TimeStamp)));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound("Operation Cancelled");
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
            }

            _logger.LogWarning(_batchLog.Print(_logId, $"Returned 0 Candle(s)"));
            return NotFound("Not Found");
        }

        //GET: api/v1/candle/bymarket?market=gbpusd&byFromDate=""
        [HttpGet("bymarket_byFromDate", Name = nameof(GetCandlesByMarketByFromDate))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CandleDto>>> GetCandlesByMarketByFromDate(string market, DateTime fromDate,
            CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"GET GetCandlesByMarketByFromDate for {market} {fromDate}");
                var marketCandles = await _unit.Candle.GetCandlesByMarketByFromDate(market, fromDate, ct);

                if (marketCandles != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId, $"Returned {marketCandles.Count} Candles(s)"));
                    return Ok(MapCandles(marketCandles.OrderBy(a=>a.TimeStamp)));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound("Operation Cancelled");
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
            }

            _logger.LogWarning(_batchLog.Print(_logId, $"Returned 0 Candle(s)"));
            return NotFound("Not Found");
        }

        //GET: api/v1/candle/bylastupdated?market=gbpusd&granularity=15
        [HttpGet("bylastupdated", Name = nameof(GetLastCandleUpdated))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DateTime>> GetLastCandleUpdated(string market, string granularity,
            CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"GET GetLastCandleUpdated for {market} {granularity}");
                var lastUpdated = await _unit.Candle.GetLastCandleUpdated(market, granularity, ct);
                _logger.LogInformation(_batchLog.Print(_logId, $"Returned {lastUpdated}"));
                return Ok(lastUpdated);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound("Operation Cancelled");
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
            }
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
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"GET GetMarketGranularityCandlesDate for {market} {granularity} {fromDate} {toDate}");
                
                if (!DateTimeOffset.TryParse(fromDate, out var fromDateOffset))
                {
                    _logger.LogWarning(_batchLog.Print(_logId, $"Incorrect FromDate format: {fromDate}"));
                    return BadRequest($"Incorrect FromDate format: {fromDate}");
                }

                if (!DateTimeOffset.TryParse(toDate, out var toDateOffset))
                {
                    _logger.LogWarning(_batchLog.Print(_logId, $"Incorrect ToDate format: {toDate}"));
                    return BadRequest($"Incorrect ToDate format: {toDate}");
                }

                var candles =
                    await _unit.Candle.GetMarketGranularityCandlesDate(market, granularity, fromDateOffset,
                        toDateOffset,
                        ct);

                if (candles != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId, $"Returned {candles.Count} Candles(s)"));
                    return Ok(MapCandles(candles.OrderBy(a=>a.TimeStamp)));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound("Operation Cancelled");
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
            }

            _logger.LogWarning(_batchLog.Print(_logId, $"Candle not found"));
            return NotFound("Not Found");
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
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"GET GetMarketGranularityCandles for {market} {granularity}");

                var candles =
                    await _unit.Candle.GetMarketGranularityCandles(market, granularity, ct);

                if (candles != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId, $"Returned {candles.Count} Candles(s)"));
                    return Ok(MapCandles(candles.OrderBy(order => order.TimeStamp)));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound("Operation Cancelled");
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
            }

            _logger.LogWarning(_batchLog.Print(_logId, $"Candle not found"));
            return NotFound("Not Found");
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
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"GET GetCandleMetrics for {market} {granularity}");
                
                var result = new CandleMetricsDto()
                {
                    MinDate = await _unit.Candle.GetFirstCandleUpdated(market, granularity, ct),
                    Quantity = await _unit.Candle.GetCandleCount(market, granularity, ct),
                    MaxDate = await _unit.Candle.GetLastCandleUpdated(market, granularity, ct)
                };

                _batchLog.Update(_logId, $"Returned {result.Quantity} {result.MinDate} {result.MaxDate}");
                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound("Operation Cancelled");
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
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
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"POST PostCandles for {candleDto.Count} Candle(s)");
                
                var candle = _mapper.Map<List<Candle>>(candleDto);

                _batchLog.Update(_logId,$"Candle Received: {candle[0].Market} {candle[0].Granularity} StartDate: {candle[0].TimeStamp} EndDate: {candle[^1].TimeStamp} Records: {candle.Count}");

                await _unit.Candle.RemoveDuplicateCandleEntries(candle, ct);
                _unit.SaveChanges();

                _batchLog.Update(_logId,"Candle Duplicates Removed");

                await _unit.Candle.AddCandlesAsync(candle, ct);
                _unit.SaveChanges();

                _batchLog.Update(_logId, "Candle Added");
                _logger.LogInformation(_batchLog.Print(_logId));

                // re-direct will not work but i wont the 201 response + records added 
                return CreatedAtAction(nameof(GetCandles), new {id = 0, version = apiVersion.ToString()}, candle);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(_batchLog.Print(_logId, $"Operation Cancelled"));
                return NotFound("Operation Cancelled");
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
                return BadRequest(_batchLog.Print(_logId, $"Error from {nameof(CandleController)}", e));
            }
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
