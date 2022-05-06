#region

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
    private readonly ITvShowRepository _tvShowRepository;

    public ShowRefresher(ITvShowRepository tvShowRepository,
                         IAddic7edClient addic7EdClient,
                         ICredentialsService credentialsService,
                         ISeasonRefresher seasonRefresher,
                         IEpisodeRefresher episodeRefresher)
    {
        _tvShowRepository = tvShowRepository;
        _addic7EdClient = addic7EdClient;
        _credentialsService = credentialsService;
        _seasonRefresher = seasonRefresher;
        _episodeRefresher = episodeRefresher;
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
        await _seasonRefresher.RefreshSeasonsAsync(tvShow, token: token);
        var show = (await _tvShowRepository.GetByIdAsync(tvShow.Id, token))!;
        foreach (var season in show.Seasons)
        {
            await _episodeRefresher.RefreshEpisodesAsync(show, season, token: token);
        }
    }

    public IAsyncEnumerable<TvShow> FindShowsAsync(string search, CancellationToken token)
    {
        return _tvShowRepository.FindAsync(search, token);
    }

    public Task<TvShow?> GetShowByIdAsync(long id, CancellationToken cancellationToken)
    {
        return _tvShowRepository.GetByIdAsync(id, cancellationToken);
    }
}