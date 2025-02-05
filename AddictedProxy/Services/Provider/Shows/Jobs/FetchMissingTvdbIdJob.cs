using AddictedProxy.Database.Model;
using AddictedProxy.Database.Repositories.Shows;
using Performance.Service;
using TvMovieDatabaseClient.Service;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public class FetchMissingTvdbIdJob
{
    private readonly ILogger<FetchMissingTvdbIdJob> _logger;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly ITMDBClient _tmdbClient;

    public FetchMissingTvdbIdJob(ILogger<FetchMissingTvdbIdJob> logger, IPerformanceTracker performanceTracker, ITvShowRepository tvShowRepository, ITMDBClient tmdbClient)
    {
        _logger = logger;
        _performanceTracker = performanceTracker;
        _tvShowRepository = tvShowRepository;
        _tmdbClient = tmdbClient;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var transaction = _performanceTracker.BeginNestedSpan("find-tvdbid");
        var count = 0;
        foreach (var show in await _tvShowRepository.GetShowsWithoutTvdbIdWithTmdbIdAsync().ToArrayAsync(cancellationToken))
        {
            var details = show.Type == ShowType.Show
                ? await _tmdbClient.GetShowExternalIdsAsync(show.TmdbId!.Value, cancellationToken)
                : await _tmdbClient.GetMovieExternalIdsAsync(show.TmdbId!.Value, cancellationToken);
            if (details == null)
            {
                _logger.LogWarning("No TVDBID for show: {ShowId}", show.TmdbId);
                continue;
            }

            show.TvdbId = details.TvdbId;
            count++;
            if (++count % 50 == 0)
            {
                _logger.LogInformation("Found TVDBID for {Count} shows", count);
                await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
            }
        }

        _logger.LogInformation("Found TVDBID for {Count} shows", count);
        await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
    }
}