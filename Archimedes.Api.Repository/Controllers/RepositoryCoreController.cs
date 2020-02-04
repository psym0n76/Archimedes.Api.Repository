using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Archimedes.Fx.Api.Repository.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositoryCoreController : ControllerBase
    {
        // GET: api/Repository
        [HttpGet]
        public IEnumerable<string> Get()
        {

            var assemblyVersion = typeof(Startup).Assembly.GetName().Version.ToString();

            return new string[] { "repository.api", "repository.api" , "version: " + assemblyVersion};
        }

        // GET: api/Repository/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Repository
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Repository/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
