using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Utils;
using TvMovieDatabaseClient.Model.Movie.Search;
using TvMovieDatabaseClient.Model.Show.Search;

namespace AddictedProxy.Services.Provider.ShowInfo;

public class ShowTmdbMapper : IShowTmdbMapper
{
    private readonly IDetailsProvider _detailsProvider;

    public ShowTmdbMapper(IDetailsProvider detailsProvider)
    {
        _detailsProvider = detailsProvider;
    }

    /// <inheritdoc />
    public async Task<ShowTmdbInfo?> TryResolveShowAsync(TvShow show, CancellationToken token)
    {
        var showInfo = await _detailsProvider.GetShowInfoAsync(show, BuildShowFilter(show), token);
        if (showInfo == default)
        {
            return null;
        }

        return new ShowTmdbInfo(
            TmdbId: showInfo.Details.Id,
            TvdbId: showInfo.ExternalIds?.TvdbId,
            IsEnded: showInfo.Details.Status == "Ended");
    }

    /// <inheritdoc />
    public async Task<MovieTmdbInfo?> TryResolveMovieAsync(TvShow show, CancellationToken token)
    {
        var movieInfo = await _detailsProvider.GetMovieInfoAsync(show, BuildMovieFilter(show), token);
        if (movieInfo == default)
        {
            return null;
        }

        return new MovieTmdbInfo(
            TmdbId: movieInfo.Details.Id,
            TvdbId: movieInfo.ExternalIds?.TvdbId);
    }

    private static Func<ShowSearchResult, bool> BuildShowFilter(TvShow show)
    {
        var dateMatch = ShowNameRegexes.ReleaseYearRegex().Match(show.Name);
        var countryMatch = ShowNameRegexes.CountryRegex().Match(show.Name);

        return searchResult =>
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
        };
    }

    private static Func<MovieSearchResult, bool> BuildMovieFilter(TvShow show)
    {
        var dateMatch = ShowNameRegexes.ReleaseYearRegex().Match(show.Name);

        return searchResult =>
        {
            if (dateMatch.Success)
            {
                return searchResult.ReleaseDate.StartsWith(dateMatch.Groups[1].Value);
            }

            return true;
        };
    }
}
