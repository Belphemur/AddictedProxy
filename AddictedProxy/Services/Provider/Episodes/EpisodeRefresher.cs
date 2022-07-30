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
using Sentry.Performance.Model;
using Sentry.Performance.Service;

namespace AddictedProxy.Services.Provider.Episodes;

public class EpisodeRefresher : IEpisodeRefresher
{
    private readonly IAddic7edClient _client;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly ISeasonRepository _seasonRepository;
    private readonly ICredentialsService _credentialsService;
    private readonly IOptions<RefreshConfig> _refreshConfig;
    private readonly ILogger<EpisodeRefresher> _logger;
    private readonly IPerformanceTracker _performanceTracker;

    public EpisodeRefresher(IAddic7edClient client,
                            IEpisodeRepository episodeRepository,
                            ISeasonRepository seasonRepository,
                            ICredentialsService credentialsService,
                            IOptions<RefreshConfig> refreshConfig,
                            ILogger<EpisodeRefresher> logger,
                            IPerformanceTracker performanceTracker
    )
    {
        _client = client;
        _episodeRepository = episodeRepository;
        _seasonRepository = seasonRepository;
        _credentialsService = credentialsService;
        _refreshConfig = refreshConfig;
        _logger = logger;
        _performanceTracker = performanceTracker;
    }

    /// <summary>
    /// Get episode. It might have been refreshed.
    /// </summary>
    /// <param name="show"></param>
    /// <param name="season"></param>
    /// <param name="episodeNumber"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<Episode?> GetRefreshEpisodeAsync(TvShow show, Season season, int episodeNumber, CancellationToken token)
    {
        await RefreshEpisodesAsync(show, season, token);

        return await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, season.Number, episodeNumber, token);
    }

    /// <summary>
    /// Refresh subtitle of a specific show and season
    /// </summary>
    /// <param name="show"></param>
    /// <param name="season"></param>
    /// <param name="token"></param>
    private async Task RefreshEpisodesAsync(TvShow show, Season season, CancellationToken token)
    {
        using var transaction = _performanceTracker.BeginNestedSpan("episode", $"refresh-episodes-subtitles for {show.Name} S{season.Number}");
        using var namedLock = Lock<EpisodeRefresher>.GetNamedLock($"{show.Id}-{season.Id}");
        if (!await namedLock.WaitAsync(TimeSpan.Zero, token))
        {
            _logger.LogInformation("Already refreshing episodes of S{season} of {show}", season.Number, show.Name);
            transaction.Finish(Status.Unavailable);
            return;
        }

        if (!IsSeasonNeedRefresh(show, season))
        {
            _logger.LogInformation("{show} S{season} don't need to have its episode refreshed", show.Name, season.Number);
            transaction.Finish(Status.Unavailable);
            return;
        }


        await using var credentials = await _credentialsService.GetLeastUsedCredsQueryingAsync(token);
        var episodes = (await _client.GetEpisodesAsync(credentials.AddictedUserCredentials, show, season.Number, token)).ToArray();
        await _episodeRepository.UpsertEpisodes(episodes, token);
        season.LastRefreshed = DateTime.UtcNow;
        await _seasonRepository.SaveChangesAsync(token);
        _logger.LogInformation("Refreshed {episodes} episodes of {show} S{season}", episodes.Length, show.Name, season.Number);
    }
    
    public bool IsSeasonNeedRefresh(TvShow show, Season season)
    {
        var refreshTime = show.Seasons.Max(s => s.Number) == season.Number ? _refreshConfig.Value.EpisodeRefresh.LastSeasonRefresh : _refreshConfig.Value.EpisodeRefresh.DefaultRefresh;
        return season.LastRefreshed == null || DateTime.UtcNow - season.LastRefreshed >= refreshTime;
    }

    /// <summary>
    /// Refresh subtitle of specific seasons of the show
    /// </summary>
    /// <param name="show"></param>
    /// <param name="seasonsToRefresh"></param>
    /// <param name="sendProgress"></param>
    /// <param name="token"></param>
    public async Task RefreshEpisodesAsync(TvShow show, IEnumerable<Season> seasonsToRefresh, Func<int, Task> sendProgress, CancellationToken token)
    {
        async Task<Episode[]?> EpisodeFetch(Season season)
        {
            using var namedLock = Lock<EpisodeRefresher>.GetNamedLock($"{show.Id}-{season.Id}");

            using var transaction = _performanceTracker.BeginNestedSpan("episodes.fetch", $"refresh-episodes-subtitles for {show.Name} S{season.Number}");

            if (!await namedLock.WaitAsync(TimeSpan.Zero, token))
            {
                _logger.LogInformation("Already refreshing episodes of S{season} of {show}", season.Number, show.Name);
                transaction.Finish(Status.Unavailable);
                return null;
            }


            if (!IsSeasonNeedRefresh(show, season))
            {
                _logger.LogInformation("{show} S{season} don't need to have its episode refreshed", show.Name, season.Number);
                transaction.Finish(Status.Unavailable);
                return null;
            }

            await using var credentials = await _credentialsService.GetLeastUsedCredsQueryingAsync(token);
            var episodes = (await _client.GetEpisodesAsync(credentials.AddictedUserCredentials, show, season.Number, token)).ToArray();
            season.LastRefreshed = DateTime.UtcNow;
            return episodes;
        }

        var seasons = seasonsToRefresh as Season[] ?? seasonsToRefresh.ToArray();
        var seasonsText = string.Join(", ", seasons.Select(season => $"S{season.Number}"));
        var results = new List<Episode[]>();
        var currentProgress = 0;
        var progressIncrement = 50 / (int)Math.Ceiling(seasons.Length / 2.0);

        using (var _ = _performanceTracker.BeginNestedSpan("episodes.seasons", $"Fetch episodes and subtitles from addic7ed for {show.Name} and Seasons {seasonsText}"))
        {
            foreach (var season in seasons.Chunk(2))
            {
                var result = await Task.WhenAll(season.Select(EpisodeFetch));
                results.AddRange(result.Where(episodes => episodes != null)!);
                currentProgress += progressIncrement;
                await sendProgress(currentProgress);
            }
        }


        using var savingSubtitlesSpan = _performanceTracker.BeginNestedSpan("episodes.save", $"Saving all the downloaded subtitles for {show.Name} and Seasons {seasonsText}");

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

        _logger.LogInformation("Refreshed {episodes} episodes of {show} {season}", total, show.Name, seasonsText);
    }
}