using AntiCaptcha.Model.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;

namespace AntiCaptcha.Service;

public class MetricGatherHostedService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<MetricGatherHostedService> _logger;
    private readonly PeriodicTimer _timer;

    // Define Prometheus metrics
    private readonly Gauge _remaining = Metrics.CreateGauge("anti_captcha_remaining_credits", "Remaining credits in USD");

    public MetricGatherHostedService(IServiceProvider services, ILogger<MetricGatherHostedService> logger)
    {
        _services = services;
        _logger = logger;
        var scrapeInterval = _services.GetRequiredService<IOptions<AntiCaptchaConfig>>().Value.ScrapeInterval;
        _timer = new PeriodicTimer(scrapeInterval);
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
            await using var scope = _services.CreateAsyncScope();
            var metrics = await scope.ServiceProvider.GetRequiredService<IAntiCaptchaClient>().GetBalanceAsync(stoppingToken);
            if (metrics is null)
            {
                _logger.LogWarning("Failed to collect metrics, received null");
                return;
            }

            _remaining.Set(metrics.Value.Balance);
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