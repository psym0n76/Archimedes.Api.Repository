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
    public class HealthController : ControllerBase
    {
        private readonly Config _config;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IOptions<Config> config, ILogger<HealthController> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Get()
        {
            _logger.LogInformation($"{_config.ApplicationName} Version: {_config.AppVersion}");
            return Ok($"{_config.ApplicationName} Version: {_config.AppVersion}");
        }
    }
}