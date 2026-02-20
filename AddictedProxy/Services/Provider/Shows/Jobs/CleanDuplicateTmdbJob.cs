using System.Text.RegularExpressions;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Provider.ShowInfo;
using AddictedProxy.Utils;
using Hangfire.Console;
using Hangfire.Server;
using Performance.Service;
using TvMovieDatabaseClient.Model.Show.Search;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public partial class CleanDuplicateTmdbJob
{
    private readonly ITvShowRepository _tvShowRepository;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ILogger<CleanDuplicateTmdbJob> _logger;
    private readonly IDetailsProvider _detailsProvider;

    public CleanDuplicateTmdbJob(ITvShowRepository tvShowRepository, IPerformanceTracker performanceTracker, ILogger<CleanDuplicateTmdbJob> logger, IDetailsProvider detailsProvider)
    {
        _tvShowRepository = tvShowRepository;
        _performanceTracker = performanceTracker;
        _logger = logger;
        _detailsProvider = detailsProvider;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        context.WriteLine("Starting to clean duplicate TMDB show entries...");
        using var span = _performanceTracker.BeginNestedSpan("dedupe-show-tmdb");
        var duplicateTvShow = await _tvShowRepository.GetDuplicateTvShowByTmdbIdAsync(token);
        _logger.LogInformation("Looking to dedupe {count} shows", duplicateTvShow.Count);
        context.WriteLine(string.Format("Found {0} groups with duplicate TMDB IDs", duplicateTvShow.Count));

        var count = 0;
        //Only works when we have only 2 that are dupe, more than that and its quite complex
        var eligibleGroups = duplicateTvShow.Where(pair => pair.Value.Length == 2).ToArray();
        var progressBar = context.WriteProgressBar();
        for (var i = 0; i < eligibleGroups.Length; i++)
        {
            var (_, shows) = eligibleGroups[i];
            if (await CheckShowWithAsync(shows, ShowNameRegexes.ReleaseYearRegex(), (result, year) => !result.FirstAirDate.StartsWith(year), token))
            {
                count++;
                progressBar.SetValue((i + 1) * 100.0 / eligibleGroups.Length);
                continue;
            }

            if (await CheckShowWithAsync(shows, ShowNameRegexes.CountryRegex(), (result, country) =>
                {
                    var cleanCountry = CountryCleanup.AddictedCountryToTmdb(country);
                    return !result.OriginCountry.Contains(cleanCountry);
                }, token))
            {
                count++;
            }
            progressBar.SetValue((i + 1) * 100.0 / eligibleGroups.Length);
        }

        _logger.LogInformation("Deduped {count} shows", count);
        context.WriteLine(string.Format("Successfully deduped {0} shows", count));
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

        var showInfo = await _detailsProvider.GetShowInfoAsync(showToUpdate, result => filter(result, showWithMatch.Match), cancellationToken);
        if (showInfo == default)
        {
            return false;
        }

        showToUpdate.TmdbId = showInfo.Details.Id;
        showToUpdate.TvdbId = showInfo.ExternalIds?.TvdbId;
        showToUpdate.IsCompleted = showInfo.Details.Status == "Ended";
        return true;
    }


}