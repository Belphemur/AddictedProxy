using ProxyScrape.Model;

namespace ProxyScrape.Service;

public interface IProxyScrapeClient
{
    /// <summary>
    /// Get proxy statistics
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException">If the request wasn't successfull</exception>
    Task<ProxyStatistics?> GetProxyStatisticsAsync(CancellationToken token);

    /// <summary>
    /// Get proxy overview
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException">If the request wasn't successfull</exception>
    Task<ProxyOverview?> GetProxyOverviewAsync(CancellationToken token);
}