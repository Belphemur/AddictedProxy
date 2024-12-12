using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;
using ProxyScrape.Model;

namespace ProxyScrape.Service;

public class MetricGatherHostedService : BackgroundService
{
    private readonly IProxyScrapeClient _proxyScrapeClient;
    private readonly IOptions<ProxyScrapeConfig> _config;
    private readonly ILogger<MetricGatherHostedService> _logger;
    private readonly PeriodicTimer _timer;

    // Define Prometheus metrics
    private readonly Gauge _used = Metrics.CreateGauge("proxy_scrape_quota_used_bytes", "Used quota in bytes", "account_id", "sub_user_id");
    private readonly Gauge _remaining = Metrics.CreateGauge("proxy_scrape_quota_remaining_bytes", "Remaining quota in bytes", "account_id", "sub_user_id");
    private readonly Gauge _scrapeTime = Metrics.CreateGauge("proxy_scrape_last_scraping_timestamp", "Last time we scraped successfully data for this account/subuser", "account_id", "sub_user_id");

    private MetricGatherHostedService(IProxyScrapeClient proxyScrapeClient, IOptions<ProxyScrapeConfig> config, ILogger<MetricGatherHostedService> logger)
    {
        _proxyScrapeClient = proxyScrapeClient;
        _config = config;
        _logger = logger;
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(15));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                await CollectMetricsAsync(stoppingToken);
            }
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
            var metrics = await _proxyScrapeClient.GetProxyStatisticsAsync(stoppingToken);
            if (metrics is null)
            {
                _logger.LogWarning( "Failed to collect metrics, received null");
                return;
            }
            _used.Labels(_config.Value.AccountId, _config.Value.SubUserId).Set(metrics.UsedData);
            _remaining.Labels(_config.Value.AccountId, _config.Value.SubUserId).Set(metrics.RemainingData);
            _scrapeTime.Labels(_config.Value.AccountId, _config.Value.SubUserId).SetToCurrentTimeUtc();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed to collect metrics");
        }
    }
}