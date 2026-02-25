using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Storage.Store.Compression;
using AsyncKeyedLock;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Locking;
using Performance.Service;

namespace AddictedProxy.Services.Provider.SeasonPack;

public class StoreSeasonPackJob
{
    private readonly ILogger<StoreSeasonPackJob> _logger;
    private readonly ICompressedStorageProvider _storageProvider;
    private readonly ISeasonPackSubtitleRepository _seasonPackSubtitleRepository;
    private readonly IPerformanceTracker _performanceTracker;
    private static readonly AsyncKeyedLocker<Guid> AsyncKeyedLocker = new(LockOptions.Default);

    public StoreSeasonPackJob(ILogger<StoreSeasonPackJob> logger, ICompressedStorageProvider storageProvider, ISeasonPackSubtitleRepository seasonPackSubtitleRepository, IPerformanceTracker performanceTracker)
    {
        _logger = logger;
        _storageProvider = storageProvider;
        _seasonPackSubtitleRepository = seasonPackSubtitleRepository;
        _performanceTracker = performanceTracker;
    }

    [Queue("store-subtitle")]
    public async Task ExecuteAsync(Guid seasonPackUniqueId, byte[] blob, PerformContext? context, CancellationToken cancellationToken)
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

        _logger.LogInformation("Saving season pack {seasonPackId} into storage", seasonPackUniqueId);
        var seasonPack = await _seasonPackSubtitleRepository.GetByUniqueIdAsync(seasonPackUniqueId, cancellationToken);
        if (seasonPack == null)
        {
            _logger.LogWarning("Season pack couldn't be found with GUID {seasonPackId}", seasonPackUniqueId);
            context?.WriteLine($"Season pack {seasonPackUniqueId} not found in database");
            return;
        }

        await using var buffer = new MemoryStream(blob);
        var storageName = $"season-packs/{seasonPack.TvShowId}/{seasonPack.Season}/{seasonPack.UniqueId}.zip";
        if (!await _storageProvider.StoreAsync(storageName, buffer, cancellationToken: cancellationToken))
        {
            context?.WriteLine($"Error: Failed to store season pack {seasonPackUniqueId} to storage");
            throw new InvalidOperationException($"Couldn't store the season pack {seasonPackUniqueId}");
        }

        seasonPack.StoragePath = storageName;
        seasonPack.StoredAt = DateTime.UtcNow;
        await _seasonPackSubtitleRepository.SaveChangeAsync(cancellationToken);
        context?.WriteLine($"Successfully stored season pack {seasonPackUniqueId} to {storageName}");
    }
}
