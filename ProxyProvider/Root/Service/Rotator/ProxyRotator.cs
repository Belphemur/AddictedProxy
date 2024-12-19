using System.Net;
using AsyncKeyedLock;
using Microsoft.Extensions.Logging;
using ProxyProvider.GeoList.Service;

namespace ProxyProvider.Root.Service.Rotator;

public class ProxyRotator(IGeoListProxyFetcher geoListProxyFetcher, ILogger<ProxyRotator> logger) : IDisposable, IProxyRotator
{
    private WebProxy[] _proxies = [];
    private int _currentProxyIndex = 0;
    private readonly AsyncKeyedLocker<string> _lock = new();


    /// <summary>
    /// Update the available proxies
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task UpdateProxiesAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating proxies");
        if (await _lock.LockOrNullAsync(nameof(ProxyRotator), TimeSpan.FromSeconds(5), cancellationToken) is null)
        {
            return;
        }
        var proxies = await geoListProxyFetcher.FetchProxiesAsync(cancellationToken);
        _proxies = proxies?.Select(proxy => new WebProxy($"{proxy.Protocols[0]}://{proxy.Ip}:{proxy.Port}")).ToArray() ?? [];
        _currentProxyIndex = 0;
        logger.LogInformation("Got {Count} proxies", _proxies.Length);
    }

    public async Task<WebProxy?> GetNextProxyAsync(CancellationToken cancellationToken)
    {
        if (await _lock.LockOrNullAsync(nameof(ProxyRotator), TimeSpan.FromSeconds(5), cancellationToken) is null)
        {
            return null;
        }

        _currentProxyIndex = (_currentProxyIndex + 1) % _proxies.Length;
        return _proxies[_currentProxyIndex];
    }


    public void Dispose()
    {
        _lock.Dispose();
    }
}