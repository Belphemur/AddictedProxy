using System.Text.RegularExpressions;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using Performance.Service;
using TvMovieDatabaseClient.Model.Mapping;
using TvMovieDatabaseClient.Model.Show;
using TvMovieDatabaseClient.Model.Show.Search;
using TvMovieDatabaseClient.Service;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public partial class CleanDuplicateTmdbJob
{
    private readonly ITvShowRepository _tvShowRepository;
    private readonly ITMDBClient _tmdbClient;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ILogger<CleanDuplicateTmdbJob> _logger;

    public CleanDuplicateTmdbJob(ITvShowRepository tvShowRepository, ITMDBClient tmdbClient, IPerformanceTracker performanceTracker, ILogger<CleanDuplicateTmdbJob> logger)
    {
        _tvShowRepository = tvShowRepository;
        _tmdbClient = tmdbClient;
        _performanceTracker = performanceTracker;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken token)
    {
        using var span = _performanceTracker.BeginNestedSpan("dedupe-show-tmdb");
        var duplicateTvShow = await _tvShowRepository.GetDuplicateTvShowByTmdbIdAsync(token);
        _logger.LogInformation("Looking to dedupe {count} shows", duplicateTvShow.Count);

        var count = 0;
        //Only works when we have only 2 that are dupe, more than that and its quite complex
        foreach (var (_, shows) in duplicateTvShow.Where(pair => pair.Value.Length == 2))
        {
            if (await CheckShowWithAsync(shows, ReleaseYearRegex(), (result, year) => !result.FirstAirDate.StartsWith(year), token))
            {
                count++;
                continue;
            }

            if (await CheckShowWithAsync(shows, CountryRegex(), (result, country) =>
                {
                    var cleanCountry = country switch
                    {
                        "UK" => "GB",
                        _    => country
                    };
                    return !result.OriginCountry.Contains(cleanCountry);
                }, token))
            {
                count++;
            }
        }
        _logger.LogInformation("Deduped {count} shows", count);
        await _tvShowRepository.BulkSaveChangesAsync(token);
    }

    private async Task<bool> CheckShowWithAsync(TvShow[] shows, Regex regex, Func<ShowSearchResult, string, bool> filter, CancellationToken cancellationToken)
    {
        using var span = _performanceTracker.BeginNestedSpan("dedupe-check");
        var showWithMatch = shows.Select(show =>
                                 {
                                     var match = regex.Match(show.Name);
                                     return !match.Success ? default : (Show: show, Match: match.Groups[1].Value);
                                 })
                                 .FirstOrDefault(s => s != default);
        if (showWithMatch == default)
        {
            return false;
        }

        var showToUpdate = shows.Single(show => show.Id != showWithMatch.Show.Id);

        var showInfo = await GetShowInfoAsync(showToUpdate, result => filter(result, showWithMatch.Match), cancellationToken);
        if (showInfo == default)
        {
            return false;
        }

        showToUpdate.TmdbId = showInfo.Details.Id;
        showToUpdate.TvdbId = showInfo.ExternalIds?.TvdbId;
        showToUpdate.IsCompleted = showInfo.Details.Status == "Ended";
        return true;
    }

    private async Task<(ShowDetails Details, ExternalIds? ExternalIds)> GetShowInfoAsync(TvShow showToUpdate, Func<ShowSearchResult, bool> filter, CancellationToken cancellationToken)
    {
        var showInfo = await _tmdbClient.SearchTvAsync(NameCleanerRegex().Replace(showToUpdate.Name, "").Replace("BBC ", ""), cancellationToken)
                                        .Where(filter)
                                        .SelectAwaitWithCancellation(async (result, token) => await _tmdbClient.GetShowDetailsByIdAsync(result.Id, token))
                                        .Where(details => details != null)
                                        .SelectAwaitWithCancellation(async (details, token) =>
                                        {
                                            var externalIds = await _tmdbClient.GetShowExternalIdsAsync(details!.Id, token);
                                            return (Details: details, ExternalIds: externalIds);
                                        })
                                        .FirstOrDefaultAsync(cancellationToken);
        return showInfo;
    }


    [GeneratedRegex(@"\((\d{4})\)", RegexOptions.Compiled)]
    private static partial Regex ReleaseYearRegex();

    [GeneratedRegex(@"\(([A-Z]{2})\)", RegexOptions.Compiled)]
    private static partial Regex CountryRegex();

    [GeneratedRegex("\\s[\\(\\[].+[\\)\\]]", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex NameCleanerRegex();
}