using System.Threading;
using System.Threading.Tasks;
using AddictedProxy.Model.Config;
using AddictedProxy.Services.Addic7ed;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace AddictedProxy.Controllers
{
    [ApiController]
    [Route("addic7ed")]
    public class Addic7ed : Controller
    {
        private readonly IAddic7edClient _client;

        public Addic7ed(IAddic7edClient client)
        {
            _client = client;
        }

        [Route("shows")]
        [HttpPost]
        public async Task<IActionResult> GetShows([FromBody]Addic7edCreds credentials, CancellationToken token)
        {
            var shows = await _client.GetTvShowsAsync(credentials, token);
            return Ok(shows);
        }
    }
}