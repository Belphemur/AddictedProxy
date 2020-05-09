using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AddictedProxy.Services.Caching;

namespace AddictedProxy.Services.Proxy
{
    public class ProxyGetter : IProxyGetter
    {
        private readonly HttpClient      _httpClient;
        private readonly ICachingService _cachingService;

        public ProxyGetter(HttpClient httpClient, ICachingService cachingService)
        {
            _httpClient             = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.proxyscrape.com");
            _cachingService         = cachingService;
        }

        /// <summary>
        /// Get list of web proxies
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<WebProxy>> GetWebProxiesAsync(CancellationToken cancellationToken)
        {
            return await _cachingService.GetSetAsync<IEnumerable<WebProxy>>("proxies", async _ =>
            {
                using var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "?request=getproxies&proxytype=http&timeout=1100&country=all&ssl=yes&anonymity=all"), cancellationToken);
                using var       textReader = new StreamReader(await response.Content.ReadAsStreamAsync());
                string          line;

                var proxies = new HashSet<WebProxy>();

                while ((line = await textReader.ReadLineAsync()) != null)
                {
                    proxies.Add(new WebProxy(new Uri($"http://{line}")));
                }

                return proxies;
            }, TimeSpan.FromMinutes(5), cancellationToken);
        }
    }
}