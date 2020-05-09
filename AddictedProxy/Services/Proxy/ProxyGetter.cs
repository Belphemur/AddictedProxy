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
            _httpClient.BaseAddress = new Uri("https://api.proxyscrape.com/");
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
                await using var stream     = await _httpClient.GetStreamAsync("?request=getproxies&proxytype=http&timeout=450&country=all&ssl=all&anonymity=all");
                using var       textReader = new StreamReader(stream);
                string          line;

                var proxies = new HashSet<WebProxy>();

                while ((line = await textReader.ReadLineAsync()) != null)
                {
                    proxies.Add(new WebProxy(new Uri(line)));
                }

                return proxies;
            }, TimeSpan.FromDays(1), cancellationToken);
        }
    }
}