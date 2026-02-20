#region

using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Provider.Merging;
using AddictedProxy.Services.Provider.Merging.Model;
using AddictedProxy.Services.Provider.ShowInfo;
using AddictedProxy.Services.Provider.Shows.Hub;
using AddictedProxy.Upstream.Service;
using Microsoft.Extensions.Logging;
using Performance.Service;

#endregion

namespace AddictedProxy.Services.Provider.Shows;

public class ShowRefresher : IShowRefresher
{
    private readonly IAddic7edClient _addic7EdClient;
    private readonly ICredentialsService _credentialsService;
    private readonly ProviderShowRefresherFactory _providerShowRefresherFactory;
    private readonly IShowExternalIdRepository _showExternalIdRepository;
    private readonly IProviderDataIngestionService _ingestionService;
    private readonly IShowTmdbMapper _showTmdbMapper;
    private readonly ILogger<ShowRefresher> _logger;
    private readonly IRefreshHubManager _refreshHubManager;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ITvShowRepository _tvShowRepository;

    public ShowRefresher(ITvShowRepository tvShowRepository,
                         IAddic7edClient addic7EdClient,
                         ICredentialsService credentialsService,
                         ProviderShowRefresherFactory providerShowRefresherFactory,
                         IShowExternalIdRepository showExternalIdRepository,
                         IProviderDataIngestionService ingestionService,
                         IShowTmdbMapper showTmdbMapper,
                         ILogger<ShowRefresher> logger,
                         IRefreshHubManager refreshHubManager,
                         IPerformanceTracker performanceTracker)
    {
        _tvShowRepository = tvShowRepository;
        _addic7EdClient = addic7EdClient;
        _credentialsService = credentialsService;
        _providerShowRefresherFactory = providerShowRefresherFactory;
        _showExternalIdRepository = showExternalIdRepository;
        _ingestionService = ingestionService;
        _showTmdbMapper = showTmdbMapper;
        _logger = logger;
        _refreshHubManager = refreshHubManager;
        _performanceTracker = performanceTracker;
    }

    public async Task RefreshShowsAsync(CancellationToken token)
    {
        await using var credentials = await _credentialsService.GetLeastUsedCredsQueryingAsync(token);
        var transaction = _performanceTracker.BeginNestedSpan(nameof(ShowRefresher), "fetch-from-addicted");

        var addic7edShows = await _addic7EdClient.GetTvShowsAsync(credentials.AddictedUserCredentials, token).ToArrayAsync(token);
        transaction.Finish();

        using var _ = _performanceTracker.BeginNestedSpan(nameof(ShowRefresher), "merge-shows");

        // Bulk-check which Addic7ed external IDs are already mapped — single SQL query
        var allExternalIds = addic7edShows.Select(s => s.ExternalId.ToString()).ToList();
        var existingIds = await _showExternalIdRepository.GetExistingExternalIdsAsync(
            DataSource.Addic7ed, allExternalIds, token);

        var mergeCount = 0;
        var skipCount = 0;

        foreach (var addic7edShow in addic7edShows)
        {
            // Fast path: skip shows already mapped in ShowExternalId — no re-merge needed
            if (existingIds.Contains(addic7edShow.ExternalId.ToString()))
            {
                skipCount++;
                continue;
            }

            var tmdbId = addic7edShow.TmdbId;
            var tvdbId = addic7edShow.TvdbId;

            // If TMDB ID is missing, resolve it now so the merge service can match by ID
            if (!tmdbId.HasValue)
            {
                var showInfo = await _showTmdbMapper.TryResolveShowAsync(addic7edShow, token);
                if (showInfo != null)
                {
                    tmdbId = showInfo.TmdbId;
                    tvdbId ??= showInfo.TvdbId;
                }
            }

            await _ingestionService.MergeShowAsync(
                DataSource.Addic7ed,
                addic7edShow.ExternalId.ToString(),
                addic7edShow.Name,
                new ThirdPartyShowIds(TvdbId: tvdbId, ImdbId: null, TmdbId: tmdbId),
                token);

            mergeCount++;
        }

        _logger.LogInformation(
            "Merged {MergeCount} new shows from Addic7ed, skipped {SkipCount} already-known shows",
            mergeCount, skipCount);
    }

    public IAsyncEnumerable<TvShow> GetShowByTvDbIdAsync(int id, CancellationToken cancellationToken)
    {
        return _tvShowRepository.GetByTvdbIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// Refresh the seasons and episodes of the show by routing to each provider that owns it.
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="token"></param>
    public async Task RefreshShowAsync(long showId, CancellationToken token)
    {
        using var transaction = _performanceTracker.BeginNestedSpan(nameof(ShowRefresher), "refresh-show");
        var show = (await _tvShowRepository.GetByIdAsync(showId, token))!;
        var externalIds = await _showExternalIdRepository.GetByShowIdAsync(show.Id, token);

        await _refreshHubManager.SendProgressAsync(show, 1, token);

        foreach (var extId in externalIds)
        {
            var refresher = _providerShowRefresherFactory.GetService(extId.Source);
            await refresher.RefreshShowAsync(show, extId, token);
        }

        await _refreshHubManager.SendRefreshDone(show, token);
    }

    public IAsyncEnumerable<TvShow> FindShowsAsync(string search, CancellationToken token)
    {
        return _tvShowRepository.FindAsync(search);
    }

    public Task<TvShow?> GetShowByGuidAsync(Guid id, CancellationToken cancellationToken)
    {
        return _tvShowRepository.GetByGuidAsync(id, cancellationToken);
    }

}