using System.Net.Http.Json;
using ProxyProvider.GeoList.Model;

namespace ProxyProvider.GeoList.Service;

public class GeoListProxyFetcher : IGeoListProxyFetcher
{
    private readonly HttpClient _client;

    public GeoListProxyFetcher(HttpClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Fetches proxies from the proxylist.geonode.com API
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProxyInfo[]?> FetchProxiesAsync(CancellationToken cancellationToken)
    {
        var response = await _client.GetAsync("https://proxylist.geonode.com/api/proxy-list?filterUpTime=90&speed=fast&limit=500&page=1&sort_by=responseTime&sort_type=asc",
            cancellationToken);
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<Response>(cancellationToken: cancellationToken);
        return data is null ? [] : data.Data;
    }
}