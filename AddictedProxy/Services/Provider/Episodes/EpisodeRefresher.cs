using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Provider.Config;
using AddictedProxy.Upstream.Service;
using Microsoft.Extensions.Options;

namespace AddictedProxy.Services.Provider.Episodes;

public class EpisodeRefresher : IEpisodeRefresher
{
    private readonly IAddic7edClient _client;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly ISeasonRepository _seasonRepository;
    private readonly ICredentialsService _credentialsService;
    private readonly IOptions<RefreshConfig> _refreshConfig;

    public EpisodeRefresher(IAddic7edClient client,
                            IEpisodeRepository episodeRepository,
                            ISeasonRepository seasonRepository,
                            ICredentialsService credentialsService,
                            IOptions<RefreshConfig> refreshConfig)
    {
        _client = client;
        _episodeRepository = episodeRepository;
        _seasonRepository = seasonRepository;
        _credentialsService = credentialsService;
        _refreshConfig = refreshConfig;
    }

    /// <summary>
    /// Get episode. It might have been refreshed.
    /// </summary>
    /// <param name="show"></param>
    /// <param name="season"></param>
    /// <param name="episodeNumber"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<(Episode? episode, bool episodesRefreshed)> GetRefreshEpisodeAsync(TvShow show, Season season, int episodeNumber, CancellationToken token)
    {
        var episode = await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, season.Number, episodeNumber, token);

        var episodeShouldRefresh = season.LastRefreshed == null || DateTime.UtcNow - season.LastRefreshed >= _refreshConfig.Value.EpisodeRefresh;
        if (episode == null || episodeShouldRefresh)
        {
            await RefreshEpisodesAsync(show, season, true, token);
            return (await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, season.Number, episodeNumber, token), true);
        }

        return (episode, episodeShouldRefresh);
    }

    /// <summary>
    /// Refresh subtitle of a specific show and season
    /// </summary>
    /// <param name="show"></param>
    /// <param name="season"></param>
    /// <param name="forceRefresh"></param>
    /// <param name="token"></param>
    public async Task RefreshEpisodesAsync(TvShow show, Season season, bool forceRefresh, CancellationToken token)
    {
        if (!forceRefresh && season.LastRefreshed != null && DateTime.UtcNow - season.LastRefreshed <= _refreshConfig.Value.EpisodeRefresh)
        {
            return;
        }

        await using var credentials = await _credentialsService.GetLeastUsedCredsAsync(token);
        var episodes = await _client.GetEpisodesAsync(credentials.AddictedUserCredentials, show, season.Number, token);
        await _episodeRepository.UpsertEpisodes(episodes, token);
        season.LastRefreshed = DateTime.UtcNow;
        await _seasonRepository.UpdateSeasonAsync(season, token);
    }
}