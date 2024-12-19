using ProxyProvider.GeoList.Model;

namespace ProxyProvider.GeoList.Service;

public interface IGeoListProxyFetcher
{
    /// <summary>
    /// Fetches proxies from the proxylist.geonode.com API
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProxyInfo[]?> FetchProxiesAsync(CancellationToken cancellationToken);
}