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
    private readonly ITvShowRepository _tvShowRepository;

    public ShowRefresher(ITvShowRepository tvShowRepository,
                         IAddic7edClient addic7EdClient,
                         ICredentialsService credentialsService,
                         ISeasonRefresher seasonRefresher,
                         IEpisodeRefresher episodeRefresher,
                         ILogger<ShowRefresher> logger,
                         IRefreshHubManager refreshHubManager
    )
    {
        _tvShowRepository = tvShowRepository;
        _addic7EdClient = addic7EdClient;
        _credentialsService = credentialsService;
        _seasonRefresher = seasonRefresher;
        _episodeRefresher = episodeRefresher;
        _logger = logger;
        _refreshHubManager = refreshHubManager;
    }

    public async Task RefreshShowsAsync(CancellationToken token)
    {
        await using var credentials = await _credentialsService.GetLeastUsedCredsAsync(token);
        var tvShows = await _addic7EdClient.GetTvShowsAsync(credentials.AddictedUserCredentials, token).ToArrayAsync(token);

        await _tvShowRepository.UpsertRefreshedShowsAsync(tvShows, token);
    }

    /// <summary>
    /// Refresh the seasons and episodes of the show
    /// </summary>
    /// <param name="tvShow"></param>
    /// <param name="token"></param>
    public async Task RefreshShowAsync(TvShow tvShow, CancellationToken token)
    {
        await _refreshHubManager.SendProgressAsync(tvShow, 1, token);
        await _seasonRefresher.RefreshSeasonsAsync(tvShow, token: token);
        await _refreshHubManager.SendProgressAsync(tvShow, 50, token);

        var currentRefresh = 50;

        var show = (await _tvShowRepository.GetByIdAsync(tvShow.Id, token))!;
        var refreshIncrement = 50 / show.Seasons.Count;
        _logger.LogInformation("Refreshing episode for {number} seasons of {show}", show.Seasons.Count, show.Name);
        await Task.WhenAll(show.Seasons.Select(async season =>
        {
            await _episodeRefresher.RefreshEpisodesAsync(show, season, token: token);
            var refresh = Interlocked.Add(ref currentRefresh, refreshIncrement);
            await _refreshHubManager.SendProgressAsync(tvShow, refresh, token);
        }));
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