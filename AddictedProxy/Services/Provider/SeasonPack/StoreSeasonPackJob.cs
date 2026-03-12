using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Storage.Store.Compression;
using AsyncKeyedLock;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Locking;
using Performance.Service;
using SuperSubtitleClient.Service;

namespace AddictedProxy.Services.Provider.SeasonPack;

public class StoreSeasonPackJob
{
    private readonly ILogger<StoreSeasonPackJob> _logger;
    private readonly ICompressedStorageProvider _storageProvider;
    private readonly ISeasonPackSubtitleRepository _seasonPackSubtitleRepository;
    private readonly ISeasonPackCatalogService _catalogService;
    private readonly ISuperSubtitlesClient _superSubtitlesClient;
    private readonly IPerformanceTracker _performanceTracker;
    private static readonly AsyncKeyedLocker<Guid> AsyncKeyedLocker = new(LockOptions.Default);

    public StoreSeasonPackJob(ILogger<StoreSeasonPackJob> logger, ICompressedStorageProvider storageProvider, ISeasonPackSubtitleRepository seasonPackSubtitleRepository, ISeasonPackCatalogService catalogService, ISuperSubtitlesClient superSubtitlesClient, IPerformanceTracker performanceTracker)
    {
        _logger = logger;
        _storageProvider = storageProvider;
        _seasonPackSubtitleRepository = seasonPackSubtitleRepository;
        _catalogService = catalogService;
        _superSubtitlesClient = superSubtitlesClient;
        _performanceTracker = performanceTracker;
    }

    [Queue("store-subtitle")]
    public async Task StoreAsync(Guid seasonPackUniqueId, byte[] blob, PerformContext? context, CancellationToken cancellationToken)
    {
        context?.WriteLine($"Starting to store season pack {seasonPackUniqueId}");
        using var releaser = await AsyncKeyedLocker.LockOrNullAsync(seasonPackUniqueId, 0, cancellationToken).ConfigureAwait(false);

        if (releaser is null)
        {
            _logger.LogInformation("Lock already taken for season pack {seasonPackId}", seasonPackUniqueId);
            context?.WriteLine($"Lock already held for season pack {seasonPackUniqueId}, skipping");
            return;
        }

        using var span = _performanceTracker.BeginNestedSpan(nameof(StoreSeasonPackJob), "store");

        var seasonPack = await GetSeasonPackAsync(seasonPackUniqueId, context, cancellationToken);
        if (seasonPack == null)
        {
            return;
        }

        await StoreAndCatalogAsync(seasonPack, blob, context, cancellationToken);
    }

    /// <summary>
    /// Download the season pack ZIP from upstream, store it, and catalog its entries.
    /// Enqueue this overload from import/refresh jobs that don't have the blob.
    /// </summary>
    [Queue("store-subtitle")]
    public async Task DownloadAndStoreAsync(Guid seasonPackUniqueId, PerformContext? context, CancellationToken cancellationToken)
    {
        context?.WriteLine($"Starting download-and-store for season pack {seasonPackUniqueId}");
        using var releaser = await AsyncKeyedLocker.LockOrNullAsync(seasonPackUniqueId, 0, cancellationToken).ConfigureAwait(false);

        if (releaser is null)
        {
            _logger.LogInformation("Lock already taken for season pack {seasonPackId}", seasonPackUniqueId);
            context?.WriteLine($"Lock already held for season pack {seasonPackUniqueId}, skipping");
            return;
        }

        using var span = _performanceTracker.BeginNestedSpan(nameof(StoreSeasonPackJob), "download-and-store");

        var seasonPack = await GetSeasonPackAsync(seasonPackUniqueId, context, cancellationToken);
        if (seasonPack == null)
        {
            return;
        }

        if (seasonPack.StoragePath != null && await _catalogService.IsCatalogedAsync(seasonPack.Id, cancellationToken))
        {
            context?.WriteLine($"Season pack {seasonPackUniqueId} already stored and cataloged, nothing to do");
            return;
        }

        _logger.LogInformation("Downloading season pack {seasonPackId} (ExternalId: {externalId}) from upstream", seasonPackUniqueId, seasonPack.ExternalId);
        context?.WriteLine($"Downloading season pack {seasonPackUniqueId} from upstream (ExternalId: {seasonPack.ExternalId})");

        var response = await _superSubtitlesClient.DownloadSubtitleAsync(seasonPack.ExternalId.ToString(), cancellationToken: cancellationToken);
        var blob = response.Content.ToByteArray();

        if (blob.Length == 0)
        {
            _logger.LogWarning("Downloaded empty blob for season pack {seasonPackId}", seasonPackUniqueId);
            context?.WriteLine($"Error: Empty download for season pack {seasonPackUniqueId}");
            return;
        }

        await StoreAndCatalogAsync(seasonPack, blob, context, cancellationToken);
    }

    private async Task<Database.Model.Shows.SeasonPackSubtitle?> GetSeasonPackAsync(Guid seasonPackUniqueId, PerformContext? context, CancellationToken cancellationToken)
    {
        var seasonPack = await _seasonPackSubtitleRepository.GetByUniqueIdAsync(seasonPackUniqueId, cancellationToken);
        if (seasonPack == null)
        {
            _logger.LogWarning("Season pack couldn't be found with GUID {seasonPackId}", seasonPackUniqueId);
            context?.WriteLine($"Season pack {seasonPackUniqueId} not found in database");
        }

        return seasonPack;
    }

    private async Task StoreAndCatalogAsync(Database.Model.Shows.SeasonPackSubtitle seasonPack, byte[] blob, PerformContext? context, CancellationToken cancellationToken)
    {
        if (seasonPack.StoragePath == null)
        {
            await using var buffer = new MemoryStream(blob);
            var storageName = $"season-packs/{seasonPack.TvShowId}/{seasonPack.Season}/{seasonPack.UniqueId}.zip";
            if (!await _storageProvider.StoreAsync(storageName, buffer, cancellationToken: cancellationToken))
            {
                context?.WriteLine($"Error: Failed to store season pack {seasonPack.UniqueId} to storage");
                throw new InvalidOperationException($"Couldn't store the season pack {seasonPack.UniqueId}");
            }

            seasonPack.StoragePath = storageName;
            seasonPack.StoredAt = DateTime.UtcNow;
            await _seasonPackSubtitleRepository.SaveChangeAsync(cancellationToken);
            context?.WriteLine($"Successfully stored season pack {seasonPack.UniqueId} to {storageName}");
        }

        await _catalogService.CatalogAndPersistAsync(seasonPack, blob, cancellationToken);
        context?.WriteLine($"Cataloged entries for season pack {seasonPack.UniqueId}");
    }
}
