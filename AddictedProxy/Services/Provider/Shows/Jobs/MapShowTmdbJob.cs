using System.Text.RegularExpressions;
using AddictedProxy.Database.Model;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using Hangfire;
using Performance.Service;
using TvMovieDatabaseClient.Service;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public class MapShowTmdbJob
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
    
    [MaximumConcurrentExecutions(1)]
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var transaction = _performanceTracker.BeginNestedSpan("refresh", "map-tmdb-to-show");

        var count = 0;
        List<TvShow> mightBeMovie = new();
        foreach (var show in await _tvShowRepository.GetShowWithoutTmdbIdAsync().ToArrayAsync(cancellationToken))
        {
            var result = await _tmdbClient.SearchTvAsync(_nameCleaner.Replace(show.Name, "").Replace("BBC ", ""), cancellationToken).FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                mightBeMovie.Add(show);
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

        count = 0;
        foreach (var show in mightBeMovie)
        {
            var result = await _tmdbClient.SearchMovieAsync(_nameCleaner.Replace(show.Name, ""), cancellationToken).FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                continue;
            }

            var details = await _tmdbClient.GetMovieDetailsByIdAsync(result.Id, cancellationToken);
            if (details == null)
            {
                continue;
            }

            show.TmdbId = details.Id;
            show.IsCompleted = true;
            show.Type = ShowType.Movie;
            if (++count % 50 == 0)
            {
                _logger.LogInformation("Found TMDB info for {count} movies", count);
                await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
            }
        }

        _logger.LogInformation("Found TMDB info for {count} movies", count);
        await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
    }
}