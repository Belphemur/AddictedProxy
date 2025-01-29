using AddictedProxy.Database.Repositories.Shows;
using Hangfire;
using Performance.Service;
using TvMovieDatabaseClient.Service;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public class CheckCompletedTmdbJob
{
    private ILogger<CheckCompletedTmdbJob> _logger;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly ITMDBClient _tmdbClient;
    private readonly IPerformanceTracker _performanceTracker;

    public CheckCompletedTmdbJob(ILogger<CheckCompletedTmdbJob> logger,
                                 ITvShowRepository tvShowRepository,
                                 ITMDBClient tmdbClient,
                                 IPerformanceTracker performanceTracker
    )
    {
        _logger = logger;
        _tvShowRepository = tvShowRepository;
        _tmdbClient = tmdbClient;
        _performanceTracker = performanceTracker;
    }

    public async Task ExecuteAsync(bool isCompleted, CancellationToken cancellationToken)
    {
        using var transaction = _performanceTracker.BeginNestedSpan("refresh", "check-show-completed");
        transaction.SetTag("completed", isCompleted);
        var count = 0;
        var showsToUpdate = new List<long>();
        foreach (var show in await _tvShowRepository.GetCompletedShows(isCompleted).ToArrayAsync(cancellationToken))
        {
            var details = await _tmdbClient.GetShowDetailsByIdAsync(show.TmdbId!.Value, cancellationToken);
            if (details == null)
            {
                continue;
            }

            if (details.Status != "Ended" && details.Status != "Canceled")
            {
                continue;
            }

            show.IsCompleted = true;
            show.LastSeasonRefreshed = null;

            showsToUpdate.Add(show.Id);


            if (++count % 50 == 0)
            {
                _logger.LogInformation("Update completed for {count} shows.", count);
                await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
            }
        }

        _logger.LogInformation("Update completed state for {count} shows", count);
        await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
        
        foreach (var showId in showsToUpdate)
        {
            BackgroundJob.Enqueue<RefreshSingleShowJob>(job => job.ExecuteAsync(showId, default));
        }
    }
}