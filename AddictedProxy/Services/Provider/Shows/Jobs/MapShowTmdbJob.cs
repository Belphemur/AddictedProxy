using AddictedProxy.Database.Model;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Provider.ShowInfo;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
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
    public async Task ExecuteAsync(PerformContext context, CancellationToken cancellationToken)
    {
        context.WriteLine("Starting TMDB mapping for shows...");
        using var transaction = _performanceTracker.BeginNestedSpan("map-tmdb-to-show");

        var count = 0;
        List<TvShow> mightBeMovie = new();
        var countNotMatched = 0;
        var shows = await _tvShowRepository.GetShowWithoutTmdbIdAsync().ToArrayAsync(cancellationToken);
        context.WriteLine($"Found {shows.Length} shows without TMDB ID");
        var progressBar = context.WriteProgressBar();
        for (var i = 0; i < shows.Length; i++)
        {
            var show = shows[i];
            var showInfo = await _showTmdbMapper.TryResolveShowAsync(show, cancellationToken);

            if (showInfo == null)
            {
                mightBeMovie.Add(show);
                countNotMatched++;
                progressBar.SetValue((i + 1) * 100.0 / shows.Length);
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
            progressBar.SetValue((i + 1) * 100.0 / shows.Length);
        }

        _logger.LogInformation("Found TMDB info for {count} shows", count);
        _logger.LogWarning("Couldn't find matching for {count} shows", countNotMatched);
        context.WriteLine($"Found TMDB info for {count} shows, {countNotMatched} unmatched");
        await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);

        count = 0;
        context.WriteLine($"Attempting to match {mightBeMovie.Count} unmatched entries as movies...");
        var movieProgressBar = context.WriteProgressBar();
        for (var i = 0; i < mightBeMovie.Count; i++)
        {
            var show = mightBeMovie[i];
            var movieInfo = await _showTmdbMapper.TryResolveMovieAsync(show, cancellationToken);

            if (movieInfo == null)
            {
                movieProgressBar.SetValue((i + 1) * 100.0 / mightBeMovie.Count);
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
            movieProgressBar.SetValue((i + 1) * 100.0 / mightBeMovie.Count);
        }

        _logger.LogInformation("Found TMDB info for {count} movies", count);
        context.WriteLine($"TMDB mapping complete: Found {count} movies");
        await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
    }
}