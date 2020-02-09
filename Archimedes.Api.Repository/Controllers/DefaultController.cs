using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Archimedes.Library.Domain;
using Microsoft.Extensions.Options;

namespace Archimedes.Api.Repository.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly Config _config;

        // GET: api/Repository
        public DefaultController(IOptions<Config> config,IUnitOfWork unit)
        {
            _config = config.Value;
        }

        [HttpGet(Name="GetDefaults")]
        public IEnumerable<string> Get()
        {
            return new string[] {$"{_config.ApplicationName} Version: {_config.AppVersion}"};
        }

        // GET: api/Repository/5
        [HttpGet("{id}", Name = "GetDefault")]
        public string Get(int id)
        {
            return "value1";
        }
    }
}
