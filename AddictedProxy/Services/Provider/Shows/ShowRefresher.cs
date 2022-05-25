#region

using AddictedProxy.Controllers.Hub;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using AddictedProxy.Services.Provider.Shows.Hub;
using AddictedProxy.Upstream.Service;
using Microsoft.AspNetCore.SignalR;
using Sentry.Performance.Service;

#endregion

namespace AddictedProxy.Services.Provider.Shows;

public class ShowRefresher : IShowRefresher
{
    private readonly IAddic7edClient _addic7EdClient;
    private readonly ICredentialsService _credentialsService;
    private readonly ISeasonRefresher _seasonRefresher;
    private readonly IEpisodeRefresher _episodeRefresher;
    private readonly ILogger<ShowRefresher> _logger;
    private readonly IRefreshHubManager _refreshHubManager;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ITvShowRepository _tvShowRepository;

    public ShowRefresher(ITvShowRepository tvShowRepository,
                         IAddic7edClient addic7EdClient,
                         ICredentialsService credentialsService,
                         ISeasonRefresher seasonRefresher,
                         IEpisodeRefresher episodeRefresher,
                         ILogger<ShowRefresher> logger,
                         IRefreshHubManager refreshHubManager,
                         IPerformanceTracker performanceTracker
    )
    {
        _tvShowRepository = tvShowRepository;
        _addic7EdClient = addic7EdClient;
        _credentialsService = credentialsService;
        _seasonRefresher = seasonRefresher;
        _episodeRefresher = episodeRefresher;
        _logger = logger;
        _refreshHubManager = refreshHubManager;
        _performanceTracker = performanceTracker;
    }

    public async Task RefreshShowsAsync(CancellationToken token)
    {
        await using var credentials = await _credentialsService.GetLeastUsedCredsAsync(token);
        var transaction = _performanceTracker.BeginNestedSpan(nameof(ShowRefresher), "fetch-from-addicted");

        var tvShows = await _addic7EdClient.GetTvShowsAsync(credentials.AddictedUserCredentials, token).ToArrayAsync(token);
        transaction.Finish();

        using var _ = _performanceTracker.BeginNestedSpan(nameof(ShowRefresher), "save-in-db");

        await _tvShowRepository.UpsertRefreshedShowsAsync(tvShows, token);
    }

    /// <summary>
    /// Refresh the seasons and episodes of the show
    /// </summary>
    /// <param name="tvShow"></param>
    /// <param name="token"></param>
    public async Task RefreshShowAsync(TvShow tvShow, CancellationToken token)
    {
        using var transaction = _performanceTracker.BeginNestedSpan(nameof(ShowRefresher), "refresh-show");
        var progressMin = 25;
        var progressMax = 100;

        await _refreshHubManager.SendProgressAsync(tvShow, 1, token);

        var currentSeasons = tvShow.Seasons.Select(season => season.Number).ToArray();

        await _seasonRefresher.RefreshSeasonsAsync(tvShow, token: token);
        await _refreshHubManager.SendProgressAsync(tvShow, progressMin, token);

        var show = (await _tvShowRepository.GetByIdAsync(tvShow.Id, token))!;
        var seasonToSync = show.Seasons;

        // If we currently have more than one season
        if (currentSeasons.Length > 0 && seasonToSync.Count > 0)
        {
            //Only sync new seasons, be sure that whatever happen, we always sync the last season
            seasonToSync = seasonToSync.ExceptBy(currentSeasons, season => season.Number)
                                       .Append(show.Seasons.OrderByDescending(season => season.Number).First())
                                       .OrderByDescending(season => season.Number)
                                       .Distinct()
                                       .ToArray();
        }


        _logger.LogInformation("Refreshing episode for {number} seasons of {show}", seasonToSync.Count, show.Name);


        async Task SendProgress(int progress)
        {
            var refreshValue = Convert.ToInt32(Math.Ceiling(progressMin + (progressMax - progressMin) * progress / 100.0));
            await _refreshHubManager.SendProgressAsync(tvShow, refreshValue, token);
        }

        await _episodeRefresher.RefreshEpisodesAsync(show, seasonToSync, SendProgress, token);
        await _refreshHubManager.SendRefreshDone(show, token);
    }

    public IAsyncEnumerable<TvShow> FindShowsAsync(string search, CancellationToken token)
    {
        return _tvShowRepository.FindAsync(search, token);
    }

    public Task<TvShow?> GetShowByGuidAsync(Guid id, CancellationToken cancellationToken)
    {
        return _tvShowRepository.GetByGuidAsync(id, cancellationToken);
    }
}