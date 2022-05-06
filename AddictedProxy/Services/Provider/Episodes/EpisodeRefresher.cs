using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Upstream.Service;

namespace AddictedProxy.Services.Provider.Episodes;

public class EpisodeRefresher : IEpisodeRefresher
{
    private readonly IAddic7edClient _client;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly ISeasonRepository _seasonRepository;
    private readonly ICredentialsService _credentialsService;

    public EpisodeRefresher(IAddic7edClient client, IEpisodeRepository episodeRepository, ISeasonRepository seasonRepository, ICredentialsService credentialsService)
    {
        _client = client;
        _episodeRepository = episodeRepository;
        _seasonRepository = seasonRepository;
        _credentialsService = credentialsService;
    }

    /// <summary>
    /// Get episode. It might have been refreshed.
    /// </summary>
    /// <param name="show"></param>
    /// <param name="season"></param>
    /// <param name="episodeNumber"></param>
    /// <param name="timeBetweenChecks"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<(Episode? episode, bool episodesRefreshed)> GetRefreshEpisodeAsync(TvShow show, Season season, int episodeNumber, TimeSpan timeBetweenChecks, CancellationToken token)
    {
        var episode = await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, season.Number, episodeNumber, token);

        var episodesRefreshed = season.LastRefreshed != null && DateTime.UtcNow - season.LastRefreshed <= timeBetweenChecks;
        if (episode == null && !episodesRefreshed)
        {
            await RefreshSubtitlesAsync(show, season, token);
            return (await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, season.Number, episodeNumber, token), true);
        }

        return (episode, episodesRefreshed);
    }

    /// <summary>
    /// Refresh subtitle of a specific show and season
    /// </summary>
    /// <param name="show"></param>
    /// <param name="season"></param>
    /// <param name="token"></param>
    public async Task RefreshSubtitlesAsync(TvShow show, Season season, CancellationToken token)
    {
        await using var credentials = await _credentialsService.GetLeastUsedCredsAsync(token);
        var episodes = await _client.GetEpisodesAsync(credentials.AddictedUserCredentials, show, season.Number, token);
        await _episodeRepository.UpsertEpisodes(episodes, token);
        season.LastRefreshed = DateTime.UtcNow;
        await _seasonRepository.UpdateSeasonAsync(season, token);
    }
}