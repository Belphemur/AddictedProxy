using System.Collections.Concurrent;
using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Provider.Config;
using AddictedProxy.Services.Provider.Seasons;
using AddictedProxy.Upstream.Service;
using Locking;
using Microsoft.Extensions.Options;

namespace AddictedProxy.Services.Provider.Episodes;

public class EpisodeRefresher : IEpisodeRefresher
{
    private readonly IAddic7edClient _client;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly ISeasonRepository _seasonRepository;
    private readonly ICredentialsService _credentialsService;
    private readonly IOptions<RefreshConfig> _refreshConfig;
    private readonly ILogger<EpisodeRefresher> _logger;

    public EpisodeRefresher(IAddic7edClient client,
                            IEpisodeRepository episodeRepository,
                            ISeasonRepository seasonRepository,
                            ICredentialsService credentialsService,
                            IOptions<RefreshConfig> refreshConfig,
                            ILogger<EpisodeRefresher> logger
    )
    {
        _client = client;
        _episodeRepository = episodeRepository;
        _seasonRepository = seasonRepository;
        _credentialsService = credentialsService;
        _refreshConfig = refreshConfig;
        _logger = logger;
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
        using var namedLock = Lock<EpisodeRefresher>.GetNamedLock($"{show.Id}-{season.Id}");
        if (!await namedLock.WaitAsync(TimeSpan.Zero, token))
        {
            _logger.LogInformation("Already refreshing episodes of S{season} of {show}", season.Number, show.Name);
            return;
        }

        if (!forceRefresh && season.LastRefreshed != null && DateTime.UtcNow - season.LastRefreshed <= _refreshConfig.Value.EpisodeRefresh)
        {
            _logger.LogInformation("{show} S{season} don't need to have its episode refreshed", show.Name, season.Number);
            return;
        }

        await using var credentials = await _credentialsService.GetLeastUsedCredsAsync(token);
        var episodes = (await _client.GetEpisodesAsync(credentials.AddictedUserCredentials, show, season.Number, token)).ToArray();
        await _episodeRepository.UpsertEpisodes(episodes, token);
        season.LastRefreshed = DateTime.UtcNow;
        await _seasonRepository.SaveChangesAsync(token);
        _logger.LogInformation("Refreshed {episodes} episodes of {show} S{season}", episodes.Length, show.Name, season.Number);
    }

    /// <summary>
    /// Refresh subtitle of specific seasons of the show
    /// </summary>
    /// <param name="show"></param>
    /// <param name="seasons"></param>
    /// <param name="sendProgress"></param>
    /// <param name="token"></param>
    public async Task RefreshEpisodesAsync(TvShow show, Season[] seasons, Func<int, Task> sendProgress, CancellationToken token)
    {
        async Task<Episode[]?> EpisodeFetch(Season season)
        {
            using var namedLock = Lock<EpisodeRefresher>.GetNamedLock($"{show.Id}-{season.Id}");


            if (!await namedLock.WaitAsync(TimeSpan.Zero, token))
            {
                _logger.LogInformation("Already refreshing episodes of S{season} of {show}", season.Number, show.Name);
                return null;
            }

            if (season.LastRefreshed != null && DateTime.UtcNow - season.LastRefreshed <= _refreshConfig.Value.EpisodeRefresh)
            {
                _logger.LogInformation("{show} S{season} don't need to have its episode refreshed", show.Name, season.Number);
                return null;
            }

            await using var credentials = await _credentialsService.GetLeastUsedCredsAsync(token);
            var episodes = (await _client.GetEpisodesAsync(credentials.AddictedUserCredentials, show, season.Number, token)).ToArray();
            season.LastRefreshed = DateTime.UtcNow;
            return episodes;
        }

        var results = new List<Episode[]>();
        var currentProgress = 0;
        var progressIncrement = 50 / (int)Math.Ceiling(seasons.Length / 2.0);


        foreach (var season in seasons.Chunk(2))
        {
            var result = await Task.WhenAll(season.Select(EpisodeFetch));
            results.AddRange(result.Where(episodes => episodes != null)!);
            currentProgress += progressIncrement;
            await sendProgress(currentProgress);
        }

        var total = 0;
        progressIncrement = 50 / (results.Count != 0 ? results.Count : 1);
        foreach (var episodes in results)
        {
            total += episodes.Length;
            await _episodeRepository.UpsertEpisodes(episodes, token);
            currentProgress += progressIncrement;
            await sendProgress(currentProgress);
        }

        await _seasonRepository.SaveChangesAsync(token);
        //Send the 100 progress if it wasn't sent before because the number was like 97%
        if (currentProgress < 100)
        {
            await sendProgress(100);
        }

        _logger.LogInformation("Refreshed {episodes} episodes of {show} {season}", total, show.Name, string.Join(", ", seasons.Select(season => $"S{season.Number}")));
    }
}