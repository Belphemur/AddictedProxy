#region

using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Storage.Store.Compression;
using AsyncKeyedLock;
using Hangfire;
using Locking;
using Performance.Service;

#endregion

namespace AddictedProxy.Services.Provider.Subtitle.Jobs;

public class StoreSubtitleJob
{
    private readonly ILogger<StoreSubtitleJob> _logger;
    private readonly ICompressedStorageProvider _storageProvider;
    private readonly ISubtitleRepository _subtitleRepository;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly static AsyncKeyedLocker<Guid> _asyncKeyedLocker = new(LockOptions.Default);

    public StoreSubtitleJob(ILogger<StoreSubtitleJob> logger, ICompressedStorageProvider storageProvider, ISubtitleRepository subtitleRepository, IPerformanceTracker performanceTracker)
    {
        _logger = logger;
        _storageProvider = storageProvider;
        _subtitleRepository = subtitleRepository;
        _performanceTracker = performanceTracker;
    }


    [Queue("store-subtitle")]
    public async Task ExecuteAsync(Guid subtitleId, byte[] subtitleBlob, CancellationToken cancellationToken)
    {
        using var releaser = await _asyncKeyedLocker.LockOrNullAsync(subtitleId, 0, cancellationToken).ConfigureAwait(false);

        if (releaser is null)
        {
            _logger.LogInformation("Lock already taken for {subtitleId}", subtitleId);
            return;
        }

        using var span = _performanceTracker.BeginNestedSpan(nameof(StoreSubtitleJob), "store");

        _logger.LogInformation("Saving subtitle {subtitleId} into the storage", subtitleId);
        var subtitle = await _subtitleRepository.GetSubtitleByGuidAsync(subtitleId, true, false, cancellationToken);
        if (subtitle == null)
        {
            _logger.LogWarning("Subtitle couldn't be found with GUID {subtitleId}", subtitleId);
            return;
        }

        await using var buffer = new MemoryStream(subtitleBlob);
        var storageName = GetStorageName(subtitle);
        if (!await _storageProvider.StoreAsync(storageName, buffer, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException($"Couldn't store the subtitle {subtitleId}");
        }

        subtitle.StoragePath = storageName;
        subtitle.StoredAt = DateTime.UtcNow;
        await _subtitleRepository.SaveChangeAsync(cancellationToken);
    }

    private string GetStorageName(Database.Model.Shows.Subtitle subtitle)
    {
        return $"{subtitle.Episode.TvShowId}/{subtitle.Episode.Season}/{subtitle.Episode.Number}/{subtitle.UniqueId}.srt";
    }
}