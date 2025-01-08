#region

using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using AddictedProxy.Services.Provider.Shows.Hub;
using AddictedProxy.Upstream.Service;
using Performance.Service;

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
    private readonly ISeasonRepository _seasonRepository;
    private readonly ITvShowRepository _tvShowRepository;

    public ShowRefresher(ITvShowRepository tvShowRepository,
                         IAddic7edClient addic7EdClient,
                         ICredentialsService credentialsService,
                         ISeasonRefresher seasonRefresher,
                         IEpisodeRefresher episodeRefresher,
                         ILogger<ShowRefresher> logger,
                         IRefreshHubManager refreshHubManager,
                         IPerformanceTracker performanceTracker,
                         ISeasonRepository seasonRepository
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
        _seasonRepository = seasonRepository;
    }

    public async Task RefreshShowsAsync(CancellationToken token)
    {
        await using var credentials = await _credentialsService.GetLeastUsedCredsQueryingAsync(token);
        var transaction = _performanceTracker.BeginNestedSpan(nameof(ShowRefresher), "fetch-from-addicted");

        var tvShows = await _addic7EdClient.GetTvShowsAsync(credentials.AddictedUserCredentials, token).ToArrayAsync(token);
        transaction.Finish();

        using var _ = _performanceTracker.BeginNestedSpan(nameof(ShowRefresher), "save-in-db");

        await _tvShowRepository.UpsertRefreshedShowsAsync(tvShows, token);
    }

    public IAsyncEnumerable<TvShow> GetShowByTvDbIdAsync(int id, CancellationToken cancellationToken)
    {
        return _tvShowRepository.GetByTvdbIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// Refresh the seasons and episodes of the show
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="token"></param>
    public async Task RefreshShowAsync(long showId, CancellationToken token)
    {
        
        using var transaction = _performanceTracker.BeginNestedSpan(nameof(ShowRefresher), "refresh-show");
        var show = (await _tvShowRepository.GetByIdAsync(showId, token))!;

        var progressMin = 25;
        var progressMax = 100;

        await _refreshHubManager.SendProgressAsync(show, 1, token);
        await _seasonRefresher.RefreshSeasonsAsync(show, token: token);
        await _refreshHubManager.SendProgressAsync(show, progressMin, token);

        var seasonToSync = await _seasonRepository.GetSeasonsForShowAsync(show.Id).OrderByDescending(season => season.Number).ToArrayAsync(cancellationToken: token);

        _logger.LogInformation("Refreshing episode for {number} seasons of {show}", seasonToSync.Length, show.Name);

        async Task SendProgress(int progress)
        {
            var refreshValue = Convert.ToInt32(Math.Ceiling(progressMin + (progressMax - progressMin) * progress / 100.0));
            await _refreshHubManager.SendProgressAsync(show, refreshValue, token);
        }

        await _episodeRefresher.RefreshEpisodesAsync(show, seasonToSync, SendProgress, token);
        var showReloaded = (await _tvShowRepository.GetByIdAsync(showId, token))!;
        await _refreshHubManager.SendRefreshDone(showReloaded, token);
    }

    public IAsyncEnumerable<TvShow> FindShowsAsync(string search, CancellationToken token)
    {
        return _tvShowRepository.FindAsync(search);
    }

    public Task<TvShow?> GetShowByGuidAsync(Guid id, CancellationToken cancellationToken)
    {
        return _tvShowRepository.GetByGuidAsync(id, cancellationToken);
    }
}