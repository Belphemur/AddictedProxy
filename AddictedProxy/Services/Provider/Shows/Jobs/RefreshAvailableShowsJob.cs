#region

using System.Text.RegularExpressions;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Job.Extensions;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Sentry.Performance.Service;
using TvMovieDatabaseClient.Service;

#endregion

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public class RefreshAvailableShowsJob : IRecurringJob
{
    private readonly IShowRefresher _showRefresher;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ITMDBClient _tmdbClient;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly ILogger<RefreshAvailableShowsJob> _logger;
    private readonly Regex NameCleaner = new Regex(@"\s[\(\[].+[\)\]]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public RefreshAvailableShowsJob(IShowRefresher showRefresher, IPerformanceTracker performanceTracker, ITMDBClient tmdbClient, ITvShowRepository tvShowRepository, ILogger<RefreshAvailableShowsJob> logger)
    {
        _showRefresher = showRefresher;
        _performanceTracker = performanceTracker;
        _tmdbClient = tmdbClient;
        _tvShowRepository = tvShowRepository;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var transaction = _performanceTracker.BeginNestedSpan("refresh", "refresh-show-list");
        await _showRefresher.RefreshShowsAsync(cancellationToken);
        await MapTmdbToShowAsync(cancellationToken);
    }

    private async Task MapTmdbToShowAsync(CancellationToken cancellationToken)
    {
        using var transaction = _performanceTracker.BeginNestedSpan("refresh", "map-tmdb-to-show");

        var count = 0;
        await foreach (var show in _tvShowRepository.GetShowWithoutTmdbIdAsync().WithCancellation(cancellationToken))
        {
            var result = await _tmdbClient.SearchTvAsync(NameCleaner.Replace(show.Name, ""), cancellationToken).FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                continue;
            }

            var details = await _tmdbClient.GetShowDetailsByIdAsync(result.Id, cancellationToken);
            if (details == null)
            {
                continue;
            }

            show.TmdbId = details.Id;
            show.IsCompleted = details.Status == "Ended";
            if (++count % 100 == 0)
            {
                _logger.LogInformation("Found TMDB info for {count} shows", count);
                await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
            }
        }

        _logger.LogInformation("Found TMDB info for {count} shows", count);
        await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
    }

    public Task OnFailure(JobException exception)
    {
        _logger.LogJobException(exception, "Couldn't refresh shows");
        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new ExponentialBackoffRetry(TimeSpan.FromSeconds(30), null);
    public TimeSpan? MaxRuntime { get; } = TimeSpan.FromMinutes(15);
    public TimeSpan Delay { get; } = TimeSpan.FromDays(1);
}