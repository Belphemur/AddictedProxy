using AddictedProxy.Database.Model;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using Hangfire.Console;
using Hangfire.Server;
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

    public async Task ExecuteAsync(PerformContext context, CancellationToken cancellationToken)
    {
        context.WriteLine("Starting to fetch missing TVDB IDs...");
        using var transaction = _performanceTracker.BeginNestedSpan("find-tvdbid");
        var count = 0;
        var shows = await _tvShowRepository.GetShowsWithoutTvdbIdWithTmdbIdAsync().ToArrayAsync(cancellationToken);
        context.WriteLine(string.Format("Found {0} shows missing TVDB ID", shows.Length));
        var progressBar = context.WriteProgressBar();
        for (var i = 0; i < shows.Length; i++)
        {
            var show = shows[i];
            var details = show.Type == ShowType.Show
                ? await _tmdbClient.GetShowExternalIdsAsync(show.TmdbId!.Value, cancellationToken)
                : await _tmdbClient.GetMovieExternalIdsAsync(show.TmdbId!.Value, cancellationToken);
            if (details == null)
            {
                _logger.LogWarning("No TVDBID for show: {ShowId}", show.TmdbId);
                progressBar.SetValue((i + 1) * 100.0 / shows.Length);
                continue;
            }

            show.TvdbId = details.TvdbId;
            count++;
            if (count % 50 == 0)
            {
                _logger.LogInformation("Found TVDBID for {Count} shows", count);
                await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
            }
            progressBar.SetValue((i + 1) * 100.0 / shows.Length);
        }

        _logger.LogInformation("Found TVDBID for {Count} shows", count);
        context.WriteLine(string.Format("Completed: Found TVDBID for {0} shows", count));
        await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
    }
}