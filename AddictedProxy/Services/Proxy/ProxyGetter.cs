using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AddictedProxy.Extensions;
using AddictedProxy.Services.Caching;

namespace AddictedProxy.Services.Proxy
{
    public class ProxyGetter : IProxyGetter
    {
        private readonly HttpClient      _httpClient;
        private readonly ICachingService _cachingService;

        public ProxyGetter(HttpClient httpClient, ICachingService cachingService)
        {
            _httpClient     = httpClient;
            _cachingService = cachingService;
        }

        /// <summary>
        /// Get list of web proxies
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<WebProxy>> GetWebProxiesAsync(CancellationToken cancellationToken)
        {
            return await _cachingService.GetSetAsync("proxies", async _ =>
            {
                var result = await Task.WhenAll(GetFreshProxies(cancellationToken), GetProxyScrape(cancellationToken));

                return result[0].Union(result[1]).ToArray();
            }, TimeSpan.FromMinutes(5), cancellationToken);
        }

        private async Task<IEnumerable<WebProxy>> GetFreshProxies(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://www.freshproxies.net/ProxyList?countries_1=DE-AT-PL-CZ-SK-HU-SI-CH&countries_2=GB-IE-BE-NL-FR-LU-AD-MC-LI&always=yes&protocol=HTTPS&level=all&order=speed&frame=1H&format=txt&fields=mini&key=8AZZQsQnEvkXwmqs"
                )
            {
                Headers = {{"User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:76.0) Gecko/20100101 Firefox/76.0"}}
            };
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            return await ParseProxies(response);
        }

        private async Task<IEnumerable<WebProxy>> GetProxyScrape(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://api.proxyscrape.com/?request=getproxies&proxytype=http&timeout=5050&country=all&ssl=all&anonymity=all&uptime=100&status=alive&averagetimeout=3000&age=unlimited&score=3.9&port=all&organization=all&serialkey=YQTIY-6D6UY-S0GL9-H6T7M"
            );
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            return await ParseProxies(response);
        }

        private static async Task<IEnumerable<WebProxy>> ParseProxies(HttpResponseMessage response)
        {
            using var textReader = new StreamReader(await response.Content.ReadAsStreamAsync());
            string    line;

            var proxies = new HashSet<WebProxy>();

            while ((line = await textReader.ReadLineAsync()) != null)
            {
                if (!Uri.IsWellFormedUriString($"http://{line}", UriKind.Absolute))
                {
                    continue;
                }

                var address = new Uri($"http://{line}");
                if (address.Port == 0)
                {
                    continue;
                }

                proxies.Add(new WebProxy(address));
            }

            return proxies;
        }
    }
}