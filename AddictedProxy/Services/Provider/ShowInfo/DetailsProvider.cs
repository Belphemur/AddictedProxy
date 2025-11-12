using System.Text.RegularExpressions;
using AddictedProxy.Database.Model.Shows;
using TvMovieDatabaseClient.Model.Mapping;
using TvMovieDatabaseClient.Model.Movie;
using TvMovieDatabaseClient.Model.Movie.Search;
using TvMovieDatabaseClient.Model.Show;
using TvMovieDatabaseClient.Model.Show.Search;
using TvMovieDatabaseClient.Service;

namespace AddictedProxy.Services.Provider.ShowInfo;

public partial class DetailsProvider : IDetailsProvider
{
    private readonly ITMDBClient _tmdbClient;

    public DetailsProvider(ITMDBClient tmdbClient)
    {
        _tmdbClient = tmdbClient;
    }

    /// <summary>
    /// Get show info
    /// </summary>
    /// <param name="show"></param>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<(ShowDetails Details, ExternalIds? ExternalIds)> GetShowInfoAsync(TvShow show, Func<ShowSearchResult, bool> filter, CancellationToken cancellationToken)
    {
        var showInfo = await _tmdbClient.SearchTvAsync(CleanName(show), cancellationToken)
                                        .Where(filter)
                                        .Select(async (result, token) => await _tmdbClient.GetShowDetailsByIdAsync(result.Id, token))
                                        .Where(details => details != null)
                                        .Select(async (details, token) =>
                                        {
                                            var externalIds = await _tmdbClient.GetShowExternalIdsAsync(details!.Id, token);
                                            return (Details: details, ExternalIds: externalIds);
                                        })
                                        .FirstOrDefaultAsync(cancellationToken);
        return showInfo;
    }

    
    /// <summary>
    /// Get movie info
    /// </summary>
    /// <param name="show"></param>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<(MovieDetails Details, ExternalIds? ExternalIds)> GetMovieInfoAsync(TvShow show, Func<MovieSearchResult, bool> filter, CancellationToken cancellationToken)
    {
        var showInfo = await _tmdbClient.SearchMovieAsync(CleanName(show), cancellationToken)
                                        .Where(filter)
                                        .SelectAwaitWithCancellation(async (result, token) => await _tmdbClient.GetMovieDetailsByIdAsync(result.Id, token))
                                        .Where(details => details != null)
                                        .SelectAwaitWithCancellation(async (details, token) =>
                                        {
                                            var externalIds = await _tmdbClient.GetMovieExternalIdsAsync(details!.Id, token);
                                            return (Details: details, ExternalIds: externalIds);
                                        })
                                        .FirstOrDefaultAsync(cancellationToken);
        return showInfo;
    }

    private static string CleanName(TvShow show)
    {
        return NameCleanerRegex().Replace(show.Name, "").Replace("BBC ", "").Replace("BBC:", "");
    }


    [GeneratedRegex("\\s[\\(\\[].+[\\)\\]]", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex NameCleanerRegex();
}