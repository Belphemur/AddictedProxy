using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Provider.Config;
using AddictedProxy.Tools.Database.Transaction;
using AddictedProxy.Upstream.Service;
using AsyncKeyedLock;
using Locking;
using Microsoft.Extensions.Options;
using Performance.Model;
using Performance.Service;

namespace AddictedProxy.Services.Provider.Seasons;

/// <summary>
/// Addic7ed-specific season refresh: fetches seasons from the Addic7ed API using credentials.
/// Extracted from the original <see cref="SeasonRefresher"/> Addic7ed-specific logic.
/// </summary>
internal class Addic7edSeasonRefresher : IProviderSeasonRefresher
{
    private readonly ILogger<Addic7edSeasonRefresher> _logger;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly IAddic7edClient _addic7EdClient;
    private readonly ICredentialsService _credentialsService;
    private readonly ISeasonRepository _seasonRepository;
    private readonly IOptions<RefreshConfig> _refreshConfig;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ITransactionManager<EntityContext> _transactionManager;
    private static readonly AsyncKeyedLocker<long> AsyncKeyedLocker = new(LockOptions.Default);

    public Addic7edSeasonRefresher(ILogger<Addic7edSeasonRefresher> logger,
                                   ITvShowRepository tvShowRepository,
                                   IAddic7edClient addic7EdClient,
                                   ICredentialsService credentialsService,
                                   ISeasonRepository seasonRepository,
                                   IOptions<RefreshConfig> refreshConfig,
                                   IPerformanceTracker performanceTracker,
                                   ITransactionManager<EntityContext> transactionManager)
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

    public DataSource Enum => DataSource.Addic7ed;

    public async Task<Season?> GetRefreshSeasonAsync(TvShow show, ShowExternalId externalId, int seasonNumber, CancellationToken token)
    {
        await RefreshSeasonsAsync(show, externalId, token);
        return await _seasonRepository.GetSeasonForShowAsync(show.Id, seasonNumber, token);
    }

    public async Task RefreshSeasonsAsync(TvShow show, ShowExternalId externalId, CancellationToken token)
    {
        using var span = _performanceTracker.BeginNestedSpan("season", $"refresh-show-seasons for show {show.Name} (Addic7ed)");
        using var releaser = await AsyncKeyedLocker.LockOrNullAsync(show.Id, 0, token).ConfigureAwait(false);

        if (releaser is null)
        {
            _logger.LogInformation("Already refreshing seasons of {show}", show.Name);
            span.Finish(Status.Unavailable);
            return;
        }

        if (!IsShowNeedsRefresh(show))
        {
            _logger.LogInformation("Don't need to refresh seasons of {show}", show.Name);
            span.SetTag("show.state", "refreshed");
            return;
        }

        await using var credentials = await _credentialsService.GetLeastUsedCredsQueryingAsync(token);

        var seasons = (await _addic7EdClient.GetSeasonsAsync(credentials.AddictedUserCredentials, show, token)).ToArray();
        await _transactionManager.WrapInTransactionAsync(async () =>
        {
            await _seasonRepository.InsertNewSeasonsAsync(show.Id, seasons, token);
            show.LastSeasonRefreshed = DateTime.UtcNow;
            await _tvShowRepository.UpdateShowAsync(show, token);
            // Reload the seasons to get the new ones
            show.Seasons = await _seasonRepository.GetSeasonsForShowAsync(show.Id).ToListAsync(token);
            _logger.LogInformation("Fetched {number} seasons of {show} (Addic7ed)", seasons.Length, show.Name);
        }, token);
    }

    public bool IsShowNeedsRefresh(TvShow show)
    {
        // Don't refresh seasons of completed show that were already refreshed
        if (show.LastSeasonRefreshed != null && show.IsCompleted)
        {
            return false;
        }

        return show.LastSeasonRefreshed == null || DateTime.UtcNow - show.LastSeasonRefreshed >= _refreshConfig.Value.SeasonRefresh;
    }
}
