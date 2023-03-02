using System.Text.RegularExpressions;
using AddictedProxy.Database.Model;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Provider.ShowInfo;
using AddictedProxy.Utils;
using Hangfire;
using Performance.Service;
using TvMovieDatabaseClient.Model.Mapping;
using TvMovieDatabaseClient.Service;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public partial class MapShowTmdbJob
{
    private readonly ILogger<MapShowTmdbJob> _logger;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly IDetailsProvider _detailsProvider;
    private readonly Regex _releaseYear = ReleaseYearRegex();
    private readonly Regex _country = CountryRegex();


    public MapShowTmdbJob(ILogger<MapShowTmdbJob> logger, IPerformanceTracker performanceTracker, ITvShowRepository tvShowRepository, IDetailsProvider detailsProvider)
    {
        _logger = logger;
        _performanceTracker = performanceTracker;
        _tvShowRepository = tvShowRepository;
        _detailsProvider = detailsProvider;
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
            var dateMatch = _releaseYear.Match(show.Name);
            var countryMatch = _country.Match(show.Name);

            var showInfo = await _detailsProvider.GetShowInfoAsync(show, searchResult =>
            {
                var takeResult = true;
                if (show.Name.Contains("BBC ") || show.Name.Contains("BBC:"))
                {
                    takeResult &= searchResult.OriginCountry.Contains("GB");
                }

                if (dateMatch.Success)
                {
                    takeResult &= searchResult.FirstAirDate.StartsWith(dateMatch.Groups[1].Value);
                }
                else if (countryMatch.Success)
                {
                    var country = CountryCleanup.AddictedCountryToTmdb(countryMatch.Groups[1].Value);
                    takeResult &= searchResult.OriginCountry.Contains(country);
                }

                return takeResult;
            }, cancellationToken);


            if (showInfo == default)
            {
                mightBeMovie.Add(show);
                countNotMatched++;
                continue;
            }

            show.TmdbId = showInfo.Details.Id;
            show.IsCompleted = showInfo.Details.Status == "Ended";
            show.TvdbId = showInfo.ExternalIds?.TvdbId;


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
            var dateMatch = _releaseYear.Match(show.Name);

            var movieInfo = await _detailsProvider.GetMovieInfoAsync(show, searchResult =>
            {
                var takeResult = true;
                if (dateMatch.Success)
                {
                    takeResult &= searchResult.ReleaseDate.StartsWith(dateMatch.Groups[1].Value);
                }

                return takeResult;
            }, cancellationToken);

            show.TmdbId = movieInfo.Details.Id;
            show.IsCompleted = true;
            show.Type = ShowType.Movie;
            show.TvdbId = movieInfo.ExternalIds?.TvdbId;
            if (++count % 50 == 0)
            {
                _logger.LogInformation("Found TMDB info for {count} movies", count);
                await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
            }
        }

        _logger.LogInformation("Found TMDB info for {count} movies", count);
        await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
    }

    [GeneratedRegex(@"\((\d{4})\)", RegexOptions.Compiled)]
    private static partial Regex ReleaseYearRegex();

    [GeneratedRegex(@"\(([A-Z]{2})\)", RegexOptions.Compiled)]
    private static partial Regex CountryRegex();
}