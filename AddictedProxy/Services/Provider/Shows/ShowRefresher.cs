#region

using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
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
    private readonly ILogger<ShowRefresher> _logger;
    private readonly IRefreshHubManager _refreshHubManager;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ITvShowRepository _tvShowRepository;

    public ShowRefresher(ITvShowRepository tvShowRepository,
                         IAddic7edClient addic7EdClient,
                         ICredentialsService credentialsService,
                         ProviderShowRefresherFactory providerShowRefresherFactory,
                         IShowExternalIdRepository showExternalIdRepository,
                         ILogger<ShowRefresher> logger,
                         IRefreshHubManager refreshHubManager,
                         IPerformanceTracker performanceTracker)
    {
        _tvShowRepository = tvShowRepository;
        _addic7EdClient = addic7EdClient;
        _credentialsService = credentialsService;
        _providerShowRefresherFactory = providerShowRefresherFactory;
        _showExternalIdRepository = showExternalIdRepository;
        _logger = logger;
        _refreshHubManager = refreshHubManager;
        _performanceTracker = performanceTracker;
    }

    public async Task RefreshShowsAsync(CancellationToken token)
    {
        await using var credentials = await _credentialsService.GetLeastUsedCredsQueryingAsync(token);
        var transaction = _performanceTracker.BeginNestedSpan(nameof(ShowRefresher), "fetch-from-addicted");

        var tvShows = await _addic7EdClient.GetTvShowsAsync(credentials.AddictedUserCredentials, token).ToArrayAsync(token);
        transaction.Finish();

        using var _ = _performanceTracker.BeginNestedSpan(nameof(ShowRefresher), "save-in-db");

        await _tvShowRepository.UpsertRefreshedShowsAsync(tvShows, token);

        // Ensure ShowExternalId entries exist for all Addic7ed shows
        var showExternalIds = tvShows
            .Where(show => show.Id > 0)
            .Select(show => new ShowExternalId
            {
                TvShowId = show.Id,
                Source = DataSource.Addic7ed,
                ExternalId = show.ExternalId.ToString()
            });
        await _showExternalIdRepository.BulkUpsertAsync(showExternalIds, token);
        _logger.LogInformation("Upserted ShowExternalId entries for {Count} Addic7ed shows", tvShows.Length);
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