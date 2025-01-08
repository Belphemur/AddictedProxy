using System.Net;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using AsyncKeyedLock;
using Microsoft.Extensions.Logging;
using ProxyProvider.GeoList.Model;
using ProxyProvider.GeoList.Service;

namespace ProxyProvider.Root.Service.Rotator;

public class ProxyRotator : IDisposable, IProxyRotator
{
    private WebProxy[] _proxies = [];
    private int _currentProxyIndex = 0;
    private readonly AsyncKeyedLocker<string> _lock = new();
    private readonly ILogger<ProxyRotator> _logger;
    private readonly IGeoListProxyFetcher _geoListProxyFetcher;
    private const int BatchSize = 20;

    public ProxyRotator(IGeoListProxyFetcher geoListProxyFetcher, ILogger<ProxyRotator> logger)
    {
        _geoListProxyFetcher = geoListProxyFetcher;
        _logger = logger;
    }

    public async Task UpdateProxiesAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating proxies");
        using var lockInfo = await _lock.LockOrNullAsync(nameof(ProxyRotator), TimeSpan.FromSeconds(5), cancellationToken);
        if (lockInfo == null)
        {
            _logger.LogWarning("Failed to acquire lock to fetch proxies");
            return;
        }

        var proxies = await _geoListProxyFetcher.FetchProxiesAsync(cancellationToken);
        var webProxies = proxies?
            .OrderBy(proxy => proxy.Responinitime)
            .Where(proxy => proxy.Responinitime < 10000)
            .Select(proxy => new WebProxy($"{proxy.Protocols[0]}://{proxy.Ip}:{proxy.Port}"))
            .ToArray() ?? [];
        _proxies = await GetWorkingProxiesAsync(webProxies, cancellationToken).ToArrayAsync(cancellationToken);
        _currentProxyIndex = 0;
        _logger.LogInformation("Got {Count} proxies", _proxies.Length);
    }

    public async IAsyncEnumerable<WebProxy> GetWorkingProxiesAsync(WebProxy[] proxies, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var workingProxies = new ConcurrentBag<WebProxy>();

        for (var i = 0; i < proxies.Length; i += BatchSize)
        {
            var batch = proxies.Skip(i).Take(BatchSize).ToArray();
            var tasks = batch.Select(proxy => TestProxyAsync(proxy, workingProxies, cancellationToken));
            await Task.WhenAll(tasks);
            _logger.LogInformation("Got {Count} working proxies out of {BatchSize}", workingProxies.Count, BatchSize);

            foreach (var proxy in workingProxies)
            {
                yield return proxy;
            }
            workingProxies.Clear();
        }
    }

    private async Task TestProxyAsync(WebProxy proxy, ConcurrentBag<WebProxy> workingProxies, CancellationToken cancellationToken)
    {
        if (await IsProxyWorkingAsync(proxy, cancellationToken))
        {
            workingProxies.Add(proxy);
        }
    }

    private async Task<bool> IsProxyWorkingAsync(WebProxy proxy, CancellationToken cancellationToken)
    {
        using var handler = new HttpClientHandler
        {
            Proxy = proxy,
            UseProxy = true
        };
        using var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(20)
        };

        try
        {
            var response = await client.GetAsync("https://ifconfig.co", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<WebProxy?> GetNextProxyAsync(CancellationToken cancellationToken)
    {
        using var lockInfo = await _lock.LockOrNullAsync(nameof(ProxyRotator), TimeSpan.FromSeconds(5), cancellationToken);
        if (lockInfo == null)
        {
            _logger.LogWarning("Failed to acquire lock to get next proxy");
            return null;
        }

        if (_proxies.Length == 0) return null;

        var proxy = _proxies[_currentProxyIndex];
        _currentProxyIndex = (_currentProxyIndex + 1) % _proxies.Length;
        return proxy;
    }

    public void Dispose()
    {
        _lock.Dispose();
    }
}