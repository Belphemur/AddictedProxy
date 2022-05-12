#region

using AddictedProxy.Controllers.Hub;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using AddictedProxy.Upstream.Service;

#endregion

namespace AddictedProxy.Services.Provider.Shows;

public class ShowRefresher : IShowRefresher
{
    private readonly IAddic7edClient _addic7EdClient;
    private readonly ICredentialsService _credentialsService;
    private readonly ISeasonRefresher _seasonRefresher;
    private readonly IEpisodeRefresher _episodeRefresher;
    private readonly ILogger<ShowRefresher> _logger;
    private readonly RefreshHub _refreshHub;
    private readonly ITvShowRepository _tvShowRepository;

    public ShowRefresher(ITvShowRepository tvShowRepository,
                         IAddic7edClient addic7EdClient,
                         ICredentialsService credentialsService,
                         ISeasonRefresher seasonRefresher,
                         IEpisodeRefresher episodeRefresher,
                         ILogger<ShowRefresher> logger,
                         RefreshHub refreshHub)
    {
        _tvShowRepository = tvShowRepository;
        _addic7EdClient = addic7EdClient;
        _credentialsService = credentialsService;
        _seasonRefresher = seasonRefresher;
        _episodeRefresher = episodeRefresher;
        _logger = logger;
        _refreshHub = refreshHub;
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
        await _refreshHub.SendProgressAsync(tvShow, 1, token);
        await _seasonRefresher.RefreshSeasonsAsync(tvShow, token: token);
        await _refreshHub.SendProgressAsync(tvShow, 50, token);

        var currentRefresh = 50;

        var show = (await _tvShowRepository.GetByIdAsync(tvShow.Id, token))!;
        var refreshIncrement = 50 / show.Seasons.Count;
        _logger.LogInformation("Refreshing episode for {number} seasons of {show}", show.Seasons.Count, show.Name);
        await Task.WhenAll(show.Seasons.Select(season =>
        {
            return _episodeRefresher.RefreshEpisodesAsync(show, season, token: token).ContinueWith(_ =>
            {
                var refresh = Interlocked.Add(ref currentRefresh, refreshIncrement);
                return _refreshHub.SendProgressAsync(tvShow,refresh, token);
            }, token);
        }));
        await _refreshHub.SendRefreshDone(tvShow, token);
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