using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AddictedProxy.Services.Proxy
{
    public class HttpProxyHandler : HttpClientHandler
    {
        private readonly IProxyGetter _proxyGetter;
        private readonly Random       _random;

        public HttpProxyHandler(IProxyGetter proxyGetter, Random random)
        {
            _proxyGetter = proxyGetter;
            _random      = random;
            UseProxy     = true;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var proxies = (await _proxyGetter.GetWebProxiesAsync(cancellationToken)).ToArray();
            var proxy   = proxies[_random.Next(1, proxies.Length) - 1];
            Proxy = proxy;
            return await base.SendAsync(request, cancellationToken);
        }
    }
}