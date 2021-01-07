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
    public class PriceController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<PriceController> _logger;
        private readonly IMapper _mapper;
        private readonly BatchLog _batchLog = new BatchLog();
        private string _logId;

        public PriceController(IUnitOfWork unit, ILogger<PriceController> logger, IMapper mapper)
        {
            _unit = unit;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PriceDto>>> GetPrices(CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                var prices = await _unit.Price.GetPricesAsync(1, 100000, ct);

                if (prices != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId));
                    return Ok(MapPrices(prices));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceController \n\nErrorMessage: {e.Message} \n\nStackTrace: {e.StackTrace}"));
                return BadRequest(e.Message);
            }

            _logger.LogWarning(_batchLog.Print(_logId,"Price not found"));
            return NotFound();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PriceDto>> GetPriceAsync(int id, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                var price = await _unit.Price.GetPriceAsync(id, ct);

                if (price != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId));
                    return Ok(MapPrice(price));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceController \n\nErrorMessage: {e.Message} \n\nStackTrace: {e.StackTrace}"));
                return BadRequest(e.Message);
            }

            _logger.LogWarning(_batchLog.Print(_logId, $"Price not found for Id: {id}"));
            return NotFound();
        }

        //GET: api/v1/price/bymarket?market=gbpusd
        [HttpGet("bymarket", Name = nameof(GetMarketPricesAsync))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PriceDto>>> GetMarketPricesAsync(string market, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                var marketPrices = await _unit.Price.GetMarketPrices(market, ct);

                if (marketPrices != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId));
                    return Ok(MapPrices(marketPrices));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceController \n\nErrorMessage: {e.Message} \n\nStackTrace: {e.StackTrace}"));
                return BadRequest(e.Message);
            }

            _logger.LogError($"Price not found for market: {market}");
            return NotFound();
        }

        //GET: api/v1/price/byLastPrice_byMarket?market=gbpusd
        [HttpGet("byLastPrice_byMarket", Name = nameof(GetLastPriceByMarket))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PriceDto>> GetLastPriceByMarket(string market, CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                var price = await _unit.Price.GetLastPriceByMarket(market, ct);

                if (price != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId));
                    return Ok(MapPrice(price));
                }

            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceController \n\nErrorMessage: {e.Message} \n\nStackTrace: {e.StackTrace}"));
                return BadRequest(e.Message);
            }

            _logger.LogError($"Price not found for market: {market}");
            return NotFound();
        }

        //GET: api/v1/price/bylastupdated?market=gbpusd&granularity=15
        [HttpGet("bylastupdated", Name = nameof(GetLastUpdated))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DateTime>> GetLastUpdated(string market, string granularity,
            CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"GET Last Updated Price for Market: {market} and Granularity: {granularity}");
                
                var lastUpdated = await _unit.Price.GetLastUpdated(market, granularity, ct);

                if (lastUpdated != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId));
                    return Ok(lastUpdated);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceController \n\nErrorMessage: {e.Message} \n\nStackTrace: {e.StackTrace}"));
                return BadRequest(e.Message);
            }

            _logger.LogError($"Price not found for market: {market}.");
            return NotFound();
        }

        //GET: api/v1/price/bymarket_bygranularity_fromdate_todate?market=gbpusd&granularity=15&fromDate=2020-01-01T05:00:00&toDate=2020-01-01T05:00:00
        [HttpGet("bymarket_bygranularity_fromdate_todate", Name = nameof(GetMarketGranularityPricesDate))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PriceDto>>> GetMarketGranularityPricesDate(string market, string granularity,
            string fromDate, string toDate,
            CancellationToken ct)
        {

            try
            {
                _logId = _batchLog.Start();
                _batchLog.Update(_logId, $"Request: Get all Prices for Market: {market} Granularity: {granularity} FromDate: {fromDate} ToDate: {toDate}");

                if (!DateTimeOffset.TryParse(fromDate, out var fromDateOffset))
                {
                    _logger.LogWarning(_batchLog.Print(_logId, $"Incorrect FromDate format: {fromDate}"));
                    return BadRequest($"Incorrect FromDate format: {fromDate}");
                }

                if (!DateTimeOffset.TryParse(toDate, out var toDateOffset))
                {
                    _logger.LogWarning(_batchLog.Print(_logId, $"Incorrect ToDate format: {fromDate}"));
                    return BadRequest($"Incorrect ToDate format: {toDate}");
                }

                var prices =
                    await _unit.Price.GetMarketGranularityPricesDate(market, granularity, fromDateOffset, toDateOffset, ct);

                if (prices != null)
                {
                    _logger.LogInformation(_batchLog.Print(_logId));
                    return Ok(MapPrices(prices));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceController \n\nErrorMessage: {e.Message} \n\nStackTrace: {e.StackTrace}"));
                return BadRequest(e.Message);
            }

            _logger.LogWarning(_batchLog.Print(_logId, $"Price not found. {nameof(market)}: {market} {nameof(granularity)}: {granularity} {nameof(fromDate)}: {fromDate} {nameof(toDate)}: {toDate}"));

            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("bymarket_distinct", Name = nameof(GetMarketDistinctAsync))]
        public async Task<ActionResult<IEnumerable<PriceDto>>> GetMarketDistinctAsync(CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                var prices = await _unit.Price.GetPricesAsync(1, 100000, ct);

                if (prices != null)
                {
                    var distinctGranularity = prices.Select(a => a.Market).Distinct();
                    // create a priceDto collection
                    var priceCollection = distinctGranularity.Select(gran => new PriceDto() {Market = gran}).ToList();
                    
                    _logger.LogInformation(_batchLog.Print(_logId));
                    return Ok(priceCollection);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceController \n\nErrorMessage: {e.Message} \n\nStackTrace: {e.StackTrace}"));
                return BadRequest(e.Message);
            }

            _logger.LogWarning(_batchLog.Print(_logId, "Prices not found"));
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("hour", Name = nameof(DeletePricesAsync))]
        public async Task<ActionResult<IEnumerable<PriceDto>>> DeletePricesAsync(CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                await _unit.Price.RemovePricesOlderThanOneHour(ct);
                
                _logger.LogInformation(_batchLog.Print(_logId,"DELETED: Prices greater than 1 Hour deleted from Table"));
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceController \n\nErrorMessage: {e.Message} \n\nStackTrace: {e.StackTrace}"));
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostPrices([FromBody] IEnumerable<PriceDto> priceDto, ApiVersion apiVersion,
            CancellationToken ct)
        {
            try
            {
                _logId = _batchLog.Start();
                
                if (!priceDto.Any())
                {
                    _logger.LogWarning(_batchLog.Print(_logId,"PriceDto Empty"));
                    return BadRequest("PriceDto Empty");
                }

                var price = _mapper.Map<IList<Price>>(priceDto);

               // await _unit.Price.RemoveDuplicatePriceEntries(price, ct);
               // _unit.SaveChanges();
                await _unit.Price.AddPricesAsync(price, ct);
                _unit.SaveChanges();

                _logger.LogInformation(_batchLog.Print(_logId));
                // re-direct will not work but i wont the 201 response + records added 
                return CreatedAtAction(nameof(GetPrices), new {id = 0, version = apiVersion.ToString()}, price);
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from PriceController \n\nErrorMessage: {e.Message} \n\nStackTrace: {e.StackTrace}"));
                return BadRequest(e.Message);
            }
        }

        private PriceDto MapPrice(Price price)
        {
            return _mapper.Map<PriceDto>(price);
        }

        private IEnumerable<PriceDto> MapPrices(IEnumerable<Price> prices)
        {
            return _mapper.Map<IEnumerable<PriceDto>>(prices);
        }
    }
}
