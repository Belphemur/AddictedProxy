using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProxyProvider.Root.Model;

namespace ProxyProvider.Root.Service.Rotator;

public class ProxyUpdaterHostedService : BackgroundService
{
    private readonly IProxyRotator _proxyProvider;
    private readonly IOptions<ProxyProviderConfig> _config;
    private readonly ILogger<ProxyUpdaterHostedService> _logger;
    private readonly PeriodicTimer _timer;

    public ProxyUpdaterHostedService(IProxyRotator proxyProvider, IOptions<ProxyProviderConfig> config, ILogger<ProxyUpdaterHostedService> logger)
    {
        _proxyProvider = proxyProvider;
        _config = config;
        _logger = logger;
        _timer = new PeriodicTimer(config.Value.RefreshInterval);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            do
            {
                await _proxyProvider.UpdateProxiesAsync(stoppingToken);
            } while (await _timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Proxy updater stopped");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer.Dispose();
        }
    }

    public sealed override void Dispose()
    {
        Dispose(true);
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}