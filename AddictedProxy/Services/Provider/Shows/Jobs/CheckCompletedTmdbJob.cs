using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Job.Extensions;
using Amazon.Runtime.Internal.Util;
using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Job.Scheduler.Scheduler;
using Sentry.Performance.Service;
using TvMovieDatabaseClient.Service;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public class CheckCompletedTmdbJob : IRecurringJob
{
    private ILogger<CheckCompletedTmdbJob> _logger;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly ITMDBClient _tmdbClient;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly IJobScheduler _jobScheduler;
    private readonly IJobBuilder _jobBuilder;

    public CheckCompletedTmdbJob(ILogger<CheckCompletedTmdbJob> logger,
                                 ITvShowRepository tvShowRepository,
                                 ITMDBClient tmdbClient,
                                 IPerformanceTracker performanceTracker,
                                 IJobScheduler jobScheduler,
                                 IJobBuilder jobBuilder)
    {
        _logger = logger;
        _tvShowRepository = tvShowRepository;
        _tmdbClient = tmdbClient;
        _performanceTracker = performanceTracker;
        _jobScheduler = jobScheduler;
        _jobBuilder = jobBuilder;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var transaction = _performanceTracker.BeginNestedSpan("refresh", "check-show-completed");
        var count = 0;
        foreach (var show in await _tvShowRepository.GetNonCompletedShows().ToArrayAsync(cancellationToken))
        {
            var details = await _tmdbClient.GetShowDetailsByIdAsync(show.TmdbId!.Value, cancellationToken);
            if (details == null)
            {
                continue;
            }

            if (details.Status != "Ended")
            {
                continue;
            }
            
            show.IsCompleted = true;
            show.LastSeasonRefreshed = null;

            var job = _jobBuilder.Create<RefreshSingleShowJob>()
                                 .Configure(job => { job.Show = show; })
                                 .Build();
            _jobScheduler.ScheduleJob(job);

  

            if (++count % 50 == 0)
            {
                _logger.LogInformation("Update completed for {count} shows.", count);
                await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
            }
        }

        _logger.LogInformation("Update completed state for {count} shows", count);
        await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
    }

    public Task OnFailure(JobException exception)
    {
        _logger.LogJobException(exception, "Couldn't check completion of running shows");
        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new NoRetry();
    public TimeSpan? MaxRuntime { get; } = TimeSpan.FromMinutes(10);
    public TimeSpan Delay { get; } = TimeSpan.FromDays(1);
}