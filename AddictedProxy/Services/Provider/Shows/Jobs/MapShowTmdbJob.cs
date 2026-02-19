using AddictedProxy.Database.Model;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Provider.ShowInfo;
using Hangfire;
using Performance.Service;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public partial class MapShowTmdbJob
{
    private readonly ILogger<MapShowTmdbJob> _logger;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly IShowTmdbMapper _showTmdbMapper;

    public MapShowTmdbJob(ILogger<MapShowTmdbJob> logger, IPerformanceTracker performanceTracker, ITvShowRepository tvShowRepository, IShowTmdbMapper showTmdbMapper)
    {
        _logger = logger;
        _performanceTracker = performanceTracker;
        _tvShowRepository = tvShowRepository;
        _showTmdbMapper = showTmdbMapper;
    }

    [MaximumConcurrentExecutions(1)]
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var transaction = _performanceTracker.BeginNestedSpan("map-tmdb-to-show");

        var count = 0;
        List<TvShow> mightBeMovie = new();
        var countNotMatched = 0;
        foreach (var show in await _tvShowRepository.GetShowWithoutTmdbIdAsync().ToArrayAsync(cancellationToken))
        {
            var showInfo = await _showTmdbMapper.TryResolveShowAsync(show, cancellationToken);

            if (showInfo == null)
            {
                mightBeMovie.Add(show);
                countNotMatched++;
                continue;
            }

            show.TmdbId = showInfo.TmdbId;
            show.IsCompleted = showInfo.IsEnded;
            show.TvdbId = showInfo.TvdbId;


            if (++count % 50 == 0)
            {
                _logger.LogInformation("Found TMDB info for {count} shows", count);
                await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
            }
        }

        _logger.LogInformation("Found TMDB info for {count} shows", count);
        _logger.LogWarning("Couldn't find matching for {count} shows", countNotMatched);
        await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);

        count = 0;
        foreach (var show in mightBeMovie)
        {
            var movieInfo = await _showTmdbMapper.TryResolveMovieAsync(show, cancellationToken);

            if (movieInfo == null)
            {
                continue;
            }

            show.TmdbId = movieInfo.TmdbId;
            show.IsCompleted = true;
            show.Type = ShowType.Movie;
            show.TvdbId = movieInfo.TvdbId;
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