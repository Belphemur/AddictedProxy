using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;
using ProxyScrape.Model;

namespace ProxyScrape.Service;

public class MetricGatherHostedService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<MetricGatherHostedService> _logger;
    private readonly PeriodicTimer _timer;

    // Define Prometheus metrics
    private readonly Gauge _used = Metrics.CreateGauge("proxy_scrape_quota_used_bytes", "Used quota in bytes", "account_id", "sub_user_id");
    private readonly Gauge _remaining = Metrics.CreateGauge("proxy_scrape_quota_remaining_bytes", "Remaining quota in bytes", "account_id", "sub_user_id");
    private readonly Gauge _scrapeTime = Metrics.CreateGauge("proxy_scrape_last_scraping_timestamp", "Last time we scraped successfully data for this account/subuser", "account_id", "sub_user_id");
    private readonly Gauge _maxBytes = Metrics.CreateGauge("proxy_scrape_quota_max_bytes", "Maximum quota in bytes", "account_id", "sub_user_id");

    public MetricGatherHostedService(IServiceProvider services, ILogger<MetricGatherHostedService> logger)
    {
        _services = services;
        _logger = logger;
        var scrapeInterval = _services.GetRequiredService<IOptions<ProxyScrapeConfig>>().Value.ScrapeInterval;
        _timer = new PeriodicTimer(scrapeInterval);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var random = new Random();
        try
        {
            do
            {
                await CollectMetricsAsync(stoppingToken);
                // Add jitter delay between 500 ms to 2 seconds
                var jitterDelay = random.Next(500, 2000);
                await Task.Delay(jitterDelay, stoppingToken);
            } while (await _timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Metric collection stopped");
        }
    }

    private async Task CollectMetricsAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            cts.CancelAfter(TimeSpan.FromMinutes(5));
            await using var scope = _services.CreateAsyncScope();
            var metrics = await scope.ServiceProvider.GetRequiredService<IProxyScrapeClient>().GetProxyOverviewAsync(cts.Token);
            var config = _services.GetRequiredService<IOptions<ProxyScrapeConfig>>();
            if (metrics is null)
            {
                _logger.LogWarning("Failed to collect metrics, received null");
                return;
            }

            _used.Labels(config.Value.AccountId, config.Value.SubUserId).Set(metrics.Data.Plans[0].BytesUsed);
            _remaining.Labels(config.Value.AccountId, config.Value.SubUserId).Set(metrics.Data.Plans[0].MaxBytes - metrics.Data.Plans[0].BytesUsed);
            _scrapeTime.Labels(config.Value.AccountId, config.Value.SubUserId).SetToCurrentTimeUtc();
            _maxBytes.Labels(config.Value.AccountId, config.Value.SubUserId).Set(metrics.Data.Plans[0].MaxBytes);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed to collect metrics");
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