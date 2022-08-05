using System.Text.RegularExpressions;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Job.Extensions;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Sentry.Performance.Service;
using TvMovieDatabaseClient.Service;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public class MapShowTmdbJob : IJob
{
    private readonly ILogger<MapShowTmdbJob> _logger;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly ITMDBClient _tmdbClient;
    private readonly Regex _nameCleaner = new Regex(@"\s[\(\[].+[\)\]]", RegexOptions.Compiled | RegexOptions.IgnoreCase);


    public MapShowTmdbJob(ILogger<MapShowTmdbJob> logger, IPerformanceTracker performanceTracker, ITvShowRepository tvShowRepository, ITMDBClient tmdbClient)
    {
        _logger = logger;
        _performanceTracker = performanceTracker;
        _tvShowRepository = tvShowRepository;
        _tmdbClient = tmdbClient;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var transaction = _performanceTracker.BeginNestedSpan("refresh", "map-tmdb-to-show");

        var count = 0;
        await foreach (var show in _tvShowRepository.GetShowWithoutTmdbIdAsync().WithCancellation(cancellationToken))
        {
            var result = await _tmdbClient.SearchTvAsync(_nameCleaner.Replace(show.Name, ""), cancellationToken).FirstOrDefaultAsync(cancellationToken);
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
            if (++count % 50 == 0)
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
        _logger.LogJobException(exception, "Couldn't map shows to tmdb");
        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new ExponentialDecorrelatedJittedBackoffRetry(25, TimeSpan.FromSeconds(5));
    public TimeSpan? MaxRuntime { get; } = TimeSpan.FromMinutes(30);
}