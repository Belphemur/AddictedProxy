using System;
using System.Threading;
using System.Threading.Tasks;
using AddictedProxy.Services.Proxy;
using Microsoft.AspNetCore.Mvc;

namespace AddictedProxy.Controllers
{
    [ApiController]
    [Route("proxy")]
    public class ProxyGetter : Controller
    {
        private readonly IProxyGetter _proxyGetter;

        public ProxyGetter(IProxyGetter proxyGetter)
        {
            _proxyGetter = proxyGetter;
        }

        [Route("get")]
        [HttpGet]
        public async Task<IActionResult> ExecuteAsync(CancellationToken token)
        {
           return Ok(await _proxyGetter.GetWebProxiesAsync(token));
        }
    }
}