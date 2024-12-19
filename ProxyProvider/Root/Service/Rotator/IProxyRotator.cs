using System.Net;

namespace ProxyProvider.Root.Service.Rotator;

public interface IProxyRotator
{
    /// <summary>
    /// Update the available proxies
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task UpdateProxiesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get the next available proxy
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<WebProxy?> GetNextProxyAsync(CancellationToken cancellationToken);
}