using System.Text.RegularExpressions;
using AddictedProxy.Database.Model;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
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
    private readonly ITMDBClient _tmdbClient;
    private readonly Regex _nameCleaner = NameCleanerRegex();
    private readonly Regex _releaseYear = ReleaseYearRegex();
    private readonly Regex _country = CountryRegex();


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
        using var transaction = _performanceTracker.BeginNestedSpan("map-tmdb-to-show");

        var count = 0;
        List<TvShow> mightBeMovie = new();
        var countNotMatched = 0;
        foreach (var show in await _tvShowRepository.GetShowWithoutTmdbIdAsync().ToArrayAsync(cancellationToken))
        {
            var dateMatch = _releaseYear.Match(show.Name);
            var countryMatch = _country.Match(show.Name);

            var results = _tmdbClient.SearchTvAsync(_nameCleaner.Replace(show.Name, "").Replace("BBC ", ""), cancellationToken);

            if (show.Name.Contains("BBC "))
            {
                results = results.Where(searchResult => searchResult.OriginCountry.Contains("GB"));
            }

            if (dateMatch.Success)
            {
                results = results.Where(searchResult => searchResult.FirstAirDate.StartsWith(dateMatch.Groups[1].Value));
            }
            else if (countryMatch.Success)
            {
                var country = countryMatch.Groups[1].Value switch
                {
                    "UK" => "GB",
                    _    => countryMatch.Groups[1].Value
                };
                results = results.Where(searchResult => searchResult.OriginCountry.Contains(country));
            }

            var showInfo = await results.SelectAwaitWithCancellation(async (result, token) => await _tmdbClient.GetShowDetailsByIdAsync(result.Id, token))
                                        .Where(details => details != null)
                                        .SelectAwaitWithCancellation(async (details, token) =>
                                        {
                                            var externalIds = await _tmdbClient.GetShowExternalIdsAsync(details!.Id, token);
                                            return (Details: details, ExternalIds: externalIds);
                                        })
                                        .FirstOrDefaultAsync(cancellationToken);

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

            var results = await _tmdbClient.SearchMovieAsync(_nameCleaner.Replace(show.Name, ""), cancellationToken).ToArrayAsync(cancellationToken);
            if (results.Length == 0)
            {
                continue;
            }

            var result = results[0];
            if (dateMatch.Success)
            {
                result = results.FirstOrDefault(searchResult => searchResult.ReleaseDate.StartsWith(dateMatch.Groups[1].Value));
            }

            if (result == null)
            {
                continue;
            }

            var details = await _tmdbClient.GetMovieDetailsByIdAsync(result.Id, cancellationToken);
            if (details == null)
            {
                continue;
            }

            var externalIds = await _tmdbClient.GetMovieExternalIdsAsync(result.Id, cancellationToken);

            show.TmdbId = details.Id;
            show.IsCompleted = true;
            show.Type = ShowType.Movie;
            show.TvdbId = externalIds?.TvdbId;
            if (++count % 50 == 0)
            {
                _logger.LogInformation("Found TMDB info for {count} movies", count);
                await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
            }
        }

        _logger.LogInformation("Found TMDB info for {count} movies", count);
        await _tvShowRepository.BulkSaveChangesAsync(cancellationToken);
    }

    [GeneratedRegex("\\s[\\(\\[].+[\\)\\]]", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex NameCleanerRegex();

    [GeneratedRegex(@"\((\d{4})\)", RegexOptions.Compiled)]
    private static partial Regex ReleaseYearRegex();

    [GeneratedRegex(@"\(([A-Z]{2})\)", RegexOptions.Compiled)]
    private static partial Regex CountryRegex();
}