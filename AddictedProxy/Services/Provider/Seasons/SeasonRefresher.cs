using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Tools.Database.Transaction;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Provider.Config;
using AddictedProxy.Upstream.Service;
using Locking;
using Microsoft.Extensions.Options;
using Performance.Model;
using Performance.Service;
using AsyncKeyedLock;

namespace AddictedProxy.Services.Provider.Seasons;

public class SeasonRefresher : ISeasonRefresher
{
    private readonly ILogger<SeasonRefresher> _logger;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly IAddic7edClient _addic7EdClient;
    private readonly ICredentialsService _credentialsService;
    private readonly ISeasonRepository _seasonRepository;
    private readonly IOptions<RefreshConfig> _refreshConfig;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ITransactionManager<EntityContext> _transactionManager;
    private readonly static AsyncKeyedLocker<long> _asyncKeyedLocker = new(LockOptions.Default);

    public SeasonRefresher(ILogger<SeasonRefresher> logger,
                           ITvShowRepository tvShowRepository,
                           IAddic7edClient addic7EdClient,
                           ICredentialsService credentialsService,
                           ISeasonRepository seasonRepository,
                           IOptions<RefreshConfig> refreshConfig,
                           IPerformanceTracker performanceTracker,
                           ITransactionManager<EntityContext> transactionManager
    )
    {
        _logger = logger;
        _tvShowRepository = tvShowRepository;
        _addic7EdClient = addic7EdClient;
        _credentialsService = credentialsService;
        _seasonRepository = seasonRepository;
        _refreshConfig = refreshConfig;
        _performanceTracker = performanceTracker;
        _transactionManager = transactionManager;
    }

    public async Task<Season?> GetRefreshSeasonAsync(TvShow show, int seasonNumber, CancellationToken token)
    {
        //Check first in the show seasons if we can find it, if we can't, then trigger refresh
        //This will reduce the amount of queries we send to Addic7ed
        var season = await _seasonRepository.GetSeasonForShowAsync(show.Id, seasonNumber, token);
        if (season != null)
        {
            return season;
        }

        await RefreshSeasonsAsync(show, token);
        return await _seasonRepository.GetSeasonForShowAsync(show.Id, seasonNumber, token);
    }

    public async Task RefreshSeasonsAsync(TvShow show, CancellationToken token = default)
    {
        using var span = _performanceTracker.BeginNestedSpan("season", $"refresh-show-seasons for show {show.Name}");
        using var releaser = await _asyncKeyedLocker.LockOrNullAsync(show.Id, 0, token).ConfigureAwait(false);

        if (releaser is null)
        {
            _logger.LogInformation("Already refreshing seasons of {show}", show.Name);
            span.Finish(Status.Unavailable);
            return;
        }

        await using var credentials = await _credentialsService.GetLeastUsedCredsQueryingAsync(token);

        if (!IsShowNeedsRefresh(show))
        {
            _logger.LogInformation("Don't need to refresh seasons of {show}", show.Name);
            span.SetTag("show.state", "refreshed");
            return;
        }

        var seasons = (await _addic7EdClient.GetSeasonsAsync(credentials.AddictedUserCredentials, show, token)).ToArray();
        await _transactionManager.WrapInTransactionAsync(async () =>
        {
            await _seasonRepository.InsertNewSeasonsAsync(show.Id, seasons, token);
            show.LastSeasonRefreshed = DateTime.UtcNow;
            await _tvShowRepository.UpdateShowAsync(show, token);
            _logger.LogInformation("Fetched {number} seasons of {show}", seasons.Length, show.Name);
        }, token);
    }

    /// <summary>
    /// Does the show need to have its seasons refreshed
    /// </summary>
    /// <param name="show"></param>
    /// <returns></returns>
    public bool IsShowNeedsRefresh(TvShow show)
    {
        //Don't refresh season of completed show that were already refreshed
        if (show.LastSeasonRefreshed != null && show.IsCompleted)
        {
            return false;
        }
        return show.LastSeasonRefreshed == null || DateTime.UtcNow - show.LastSeasonRefreshed >= _refreshConfig.Value.SeasonRefresh;
    }
}