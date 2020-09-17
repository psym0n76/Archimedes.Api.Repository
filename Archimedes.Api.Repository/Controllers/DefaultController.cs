using System;
using Microsoft.AspNetCore.Mvc;
using Archimedes.Library.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Archimedes.Api.Repository.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly Config _config;
        private readonly ILogger<CandleController> _logger;

        public DefaultController(IOptions<Config> config, ILogger<CandleController> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Get()
        {
            try
            {
                _logger.LogInformation($"{_config.ApplicationName} Version: {_config.AppVersion}");
                return Ok($"{_config.ApplicationName} Version: {_config.AppVersion}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }


        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Get(int id)
        {
            try
            {
                _logger.LogInformation($"{_config.ApplicationName} Version: {_config.AppVersion}");
                return Ok($"{_config.ApplicationName} Version: {_config.AppVersion} Id:{id}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error {e.Message} {e.StackTrace}");
                return BadRequest();
            }
        }
    }
}
