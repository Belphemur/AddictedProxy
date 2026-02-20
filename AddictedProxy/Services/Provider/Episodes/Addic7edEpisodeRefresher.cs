using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Provider.Config;
using AddictedProxy.Upstream.Service;
using AsyncKeyedLock;
using Locking;
using Microsoft.Extensions.Options;
using Performance.Model;
using Performance.Service;

namespace AddictedProxy.Services.Provider.Episodes;

/// <summary>
/// Addic7ed-specific episode refresh: fetches episodes and subtitles from the Addic7ed API using credentials.
/// Extracted from the original <see cref="EpisodeRefresher"/> Addic7ed-specific logic.
/// </summary>
internal class Addic7edEpisodeRefresher : IProviderEpisodeRefresher
{
    private readonly IAddic7edClient _client;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly ISeasonRepository _seasonRepository;
    private readonly ICredentialsService _credentialsService;
    private readonly IOptions<RefreshConfig> _refreshConfig;
    private readonly ILogger<Addic7edEpisodeRefresher> _logger;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly IServiceProvider _serviceProvider;
    private static readonly AsyncKeyedLocker<(long showId, long seasonId)> AsyncKeyedLocker = new(LockOptions.Default);

    public Addic7edEpisodeRefresher(IAddic7edClient client,
                                    IEpisodeRepository episodeRepository,
                                    ISeasonRepository seasonRepository,
                                    ICredentialsService credentialsService,
                                    IOptions<RefreshConfig> refreshConfig,
                                    ILogger<Addic7edEpisodeRefresher> logger,
                                    IPerformanceTracker performanceTracker,
                                    IServiceProvider serviceProvider)
    {
        _client = client;
        _episodeRepository = episodeRepository;
        _seasonRepository = seasonRepository;
        _credentialsService = credentialsService;
        _refreshConfig = refreshConfig;
        _logger = logger;
        _performanceTracker = performanceTracker;
        _serviceProvider = serviceProvider;
    }

    public DataSource Enum => DataSource.Addic7ed;

    public async Task<Episode?> GetRefreshEpisodeAsync(TvShow show, ShowExternalId showExternalId, Season season, int episodeNumber, CancellationToken token)
    {
        await RefreshEpisodesAsync(show, season, token);
        return await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, season.Number, episodeNumber, token);
    }

    /// <summary>
    /// Refresh episodes for a single season from Addic7ed.
    /// </summary>
    private async Task RefreshEpisodesAsync(TvShow show, Season season, CancellationToken token)
    {
        using var transaction = _performanceTracker.BeginNestedSpan("episode", $"refresh-episodes-subtitles for {show.Name} S{season.Number} (Addic7ed)");
        using var releaser = await AsyncKeyedLocker.LockOrNullAsync((show.Id, season.Id), 0, token).ConfigureAwait(false);

        if (releaser is null)
        {
            _logger.LogInformation("Already refreshing episodes of S{season} of {show}", season.Number, show.Name);
            transaction.Finish(Status.Unavailable);
            return;
        }

        if (!IsSeasonNeedRefresh(show, season))
        {
            _logger.LogInformation("{show} S{season} don't need to have its episode refreshed", show.Name, season.Number);
            transaction.SetTag("season.state", "refreshed");
            return;
        }

        await using var credentials = await _credentialsService.GetLeastUsedCredsQueryingAsync(token);
        var episodes = (await _client.GetEpisodesAsync(credentials.AddictedUserCredentials, show, season.Number, token)).ToArray();
        await _episodeRepository.UpsertEpisodes(episodes, token);
        await _seasonRepository.UpdateLastRefreshedFromIdAsync(season.Id, DateTime.UtcNow, token);
        _logger.LogInformation("Refreshed {episodes} episodes of {show} S{season} (Addic7ed)", episodes.Length, show.Name, season.Number);
    }

    public bool IsSeasonNeedRefresh(TvShow show, Season season)
    {
        // Don't refresh season for completed show when it has been refreshed already before
        // and it has been less than the completed refresh delay
        if (season.LastRefreshed != null && show.IsCompleted && DateTime.UtcNow - season.LastRefreshed < _refreshConfig.Value.EpisodeRefresh.CompletedShowRefresh)
        {
            return false;
        }

        var refreshTime = show.Seasons.Max(s => s.Number) == season.Number
            ? _refreshConfig.Value.EpisodeRefresh.LastSeasonRefresh
            : _refreshConfig.Value.EpisodeRefresh.DefaultRefresh;
        return season.LastRefreshed == null || DateTime.UtcNow - season.LastRefreshed >= refreshTime;
    }

    public async Task RefreshEpisodesAsync(TvShow show, ShowExternalId showExternalId, IEnumerable<Season> seasonsToRefresh, Func<int, Task> sendProgress, CancellationToken token)
    {
        async Task<Episode[]?> EpisodeFetch(Season season)
        {
            using var transaction = _performanceTracker.BeginNestedSpan("episodes.fetch", $"refresh-episodes-subtitles for {show.Name} S{season.Number} (Addic7ed)");
            using var releaser = await AsyncKeyedLocker.LockOrNullAsync((show.Id, season.Id), 0, token).ConfigureAwait(false);

            if (releaser is null)
            {
                _logger.LogInformation("Already refreshing episodes of S{season} of {show}", season.Number, show.Name);
                transaction.Finish(Status.Unavailable);
                return null;
            }

            if (!IsSeasonNeedRefresh(show, season))
            {
                _logger.LogInformation("{show} S{season} don't need to have its episode refreshed", show.Name, season.Number);
                transaction.SetTag("season.state", "refreshed");
                return null;
            }

            await using var scope = _serviceProvider.CreateAsyncScope();

            await using var credentials = await scope.ServiceProvider.GetRequiredService<ICredentialsService>().GetLeastUsedCredsQueryingAsync(token);
            var episodes = (await _client.GetEpisodesAsync(credentials.AddictedUserCredentials, show, season.Number, token)).ToArray();
            season.LastRefreshed = DateTime.UtcNow;
            return episodes;
        }

        var seasons = seasonsToRefresh as Season[] ?? seasonsToRefresh.ToArray();
        var seasonsText = string.Join(", ", seasons.Select(season => $"S{season.Number}"));
        var results = new List<Episode[]>();
        var currentProgress = 0;
        var seasonCount = seasons.Length != 0 ? seasons.Length : 1;
        var progressIncrement = 50 / (int)Math.Ceiling(seasonCount / 2.0);

        using (_performanceTracker.BeginNestedSpan("episodes.seasons", $"Fetch episodes and subtitles from addic7ed for {show.Name} and Seasons {seasonsText}"))
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
        // Send the 100 progress if it wasn't sent before because the number was like 97%
        if (currentProgress < 100)
        {
            await sendProgress(100);
        }

        _logger.LogInformation("Refreshed {episodes} episodes of {show} {season} (Addic7ed)", total, show.Name, seasonsText);
    }

}
