using System;
using Microsoft.Extensions.Logging;
using Archimedes.Library.Domain;
using Archimedes.Library.Logger;
using Archimedes.Library.Message.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Archimedes.Api.Repository.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly Config _config;
        private readonly BatchLog _batchLog = new();
        private string _logId;

        public HealthController(ILogger<HealthController> logger, IOptions<Config> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<HealthMonitorDto> GetHealth()
        {
            _logId = _batchLog.Start();
            _batchLog.Update(_logId, $"GET GetHealth");

            var health = new HealthMonitorDto()
            {
                AppName = _config.ApplicationName,
                Version = _config.AppVersion,
                LastActiveVersion = _config.AppVersion,
                Status = true,
                LastUpdated = DateTime.Now,
                LastActive = DateTime.Now
            };

            try
            {
                _logger.LogInformation(_batchLog.Print(_logId,$"Returned {health.AppName} {health.StatusMessage}"));
                return Ok(health);
            }
            catch (Exception e)
            {
                _logger.LogError(_batchLog.Print(_logId, $"Error from HealthController", e));
                return BadRequest(e.Message);
            }
        }
    }
}