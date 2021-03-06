#region

using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Job.Extensions;
using AddictedProxy.Storage.Store;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Locking;
using Sentry;
using Sentry.Performance.Service;

#endregion

namespace AddictedProxy.Services.Provider.Subtitle.Jobs;

public class StoreSubtitleJob : IJob
{
    private readonly ILogger<StoreSubtitleJob> _logger;
    private readonly IStorageProvider _storageProvider;
    private readonly ISubtitleRepository _subtitleRepository;
    private readonly IPerformanceTracker _performanceTracker;

    public StoreSubtitleJob(ILogger<StoreSubtitleJob> logger, IStorageProvider storageProvider, ISubtitleRepository subtitleRepository, IPerformanceTracker performanceTracker)
    {
        _logger = logger;
        _storageProvider = storageProvider;
        _subtitleRepository = subtitleRepository;
        _performanceTracker = performanceTracker;
    }

    public Guid SubtitleId { get; set; }
    public byte[] SubtitleBlob { get; set; } = null!;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var namedLock = Lock<StoreSubtitleJob>.GetNamedLock(SubtitleId.ToString());
        if (!await namedLock.WaitAsync(TimeSpan.Zero, cancellationToken))
        {
            _logger.LogInformation("Lock already taken for {subtitleId}", SubtitleId);
            return;
        }

        using var span = _performanceTracker.BeginNestedSpan(nameof(StoreSubtitleJob), "store");

        _logger.LogInformation("Saving subtitle {subtitleId} into the storage", SubtitleId);
        var subtitle = await _subtitleRepository.GetSubtitleByGuidAsync(SubtitleId, true, false, cancellationToken);
        if (subtitle == null)
        {
            _logger.LogWarning("Subtitle couldn't be found with GUID {subtitleId}", SubtitleId);
            return;
        }

        await using var buffer = new MemoryStream(SubtitleBlob);
        var storageName = GetStorageName(subtitle);
        if (!await _storageProvider.StoreAsync(storageName, buffer, cancellationToken))
        {
            throw new InvalidOperationException($"Couldn't store the subtitle {SubtitleId}");
        }

        subtitle.StoragePath = storageName;
        subtitle.StoredAt = DateTime.UtcNow;
        await _subtitleRepository.SaveChangeAsync(cancellationToken);
    }

    public Task OnFailure(JobException exception)
    {
        _logger.LogJobException(exception, "Issue while saving subtitle {subtitleId} in storage", SubtitleId);
        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new ExponentialBackoffRetry(TimeSpan.FromSeconds(1), 10);
    public TimeSpan? MaxRuntime { get; } = TimeSpan.FromSeconds(30);


    private string GetStorageName(Database.Model.Shows.Subtitle subtitle)
    {
        return $"{subtitle.Episode.TvShowId}/{subtitle.Episode.Season}/{subtitle.Episode.Number}/{subtitle.UniqueId}.srt";
    }
}