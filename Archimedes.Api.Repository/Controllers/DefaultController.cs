using Microsoft.AspNetCore.Mvc;
using Archimedes.Library.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Archimedes.Api.Repository.Controllers
{
    [ApiVersion("1.0")]
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly Config _config;
        private readonly ILogger<CandleController> _logger;

        // GET: api/Repository
        public DefaultController(IOptions<Config> config, ILogger<CandleController> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet()]
        public IActionResult Get()
        {
            _logger.LogInformation($"{_config.ApplicationName} Version: {_config.AppVersion}");
            return Ok($"{_config.ApplicationName} Version: {_config.AppVersion}");

        }

        // GET: api/Repository/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            _logger.LogInformation($"{_config.ApplicationName} Version: {_config.AppVersion}");
            return Ok($"{_config.ApplicationName} Version: {_config.AppVersion}");
        }
    }
}
