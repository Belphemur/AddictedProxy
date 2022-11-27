using System;
using System.Threading;
using System.Threading.Tasks;
using AddictedProxy.Stats.Popularity.Model;
using AddictedProxy.Stats.Popularity.Service;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Stats.Popularity.Jobs;

public class RecordPopularityJob : IQueueJob
{
    private readonly ILogger<RecordPopularityJob> _logger;
    private readonly IShowPopularityService _showPopularityService;

    public RecordPopularityPayload Payload { get; set; }

    public RecordPopularityJob(ILogger<RecordPopularityJob> logger, IShowPopularityService showPopularityService)
    {
        _logger = logger;
        _showPopularityService = showPopularityService;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return _showPopularityService.RecordPopularityAsync(Payload, cancellationToken);
    }

    public Task OnFailure(JobException exception)
    {
        _logger.LogInformation("Job exception: {Message}", exception.Message);
        _logger.Log(LogLevel.Error, exception.InnerException, "Couldn't record popularity: {payload}", Payload);

        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new ExponentialDecorrelatedJittedBackoffRetry(5, TimeSpan.FromSeconds(10));
    public TimeSpan? MaxRuntime { get; } = TimeSpan.FromMinutes(2);
    public string Key => $"{Payload.TvShowId}-{Payload.Language.TwoLetterISOLanguageName}";
    public string QueueId => nameof(RecordPopularityJob);
}