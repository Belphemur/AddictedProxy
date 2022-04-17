using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Storage.Store;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Locking;

namespace AddictedProxy.Services.Provider.Subtitle.Job;

public class StoreSubtitleJob : IJob
{
    private readonly ILogger<StoreSubtitleJob> _logger;
    private readonly IStorageProvider _storageProvider;
    private readonly ISubtitleRepository _subtitleRepository;

    public Guid SubtitleId { get; set; }
    public byte[] SubtitleBlob { get; set; } = null!;

    public StoreSubtitleJob(ILogger<StoreSubtitleJob> logger, IStorageProvider storageProvider, ISubtitleRepository subtitleRepository)
    {
        _logger = logger;
        _storageProvider = storageProvider;
        _subtitleRepository = subtitleRepository;
    }


    private string GetStorageName(Database.Model.Shows.Subtitle subtitle)
        => $"{subtitle.Episode.TvShowId}/{subtitle.Episode.Season}/{subtitle.Episode.Number}/{subtitle.UniqueId}.srt";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var namedLock = Lock<StoreSubtitleJob>.GetNamedLock(SubtitleId.ToString());
        if (!await namedLock.WaitAsync(TimeSpan.Zero, cancellationToken))
        {
            _logger.LogInformation("Lock already taken for {subtitleId}", SubtitleId);
            return;
        }

        _logger.LogInformation("Saving subtitle {subtitleId} into the storage", SubtitleId);
        var subtitle = await _subtitleRepository.GetSubtitleByGuidAsync(SubtitleId, true, cancellationToken);
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
        await _subtitleRepository.UpdateAsync(subtitle, cancellationToken);
    }

    public Task OnFailure(JobException exception)
    {
        _logger.LogError(exception, "Issue while saving subtitle {subtitleId} in storage", SubtitleId);
        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new ExponentialBackoffRetry(TimeSpan.FromSeconds(1), 10);
    public TimeSpan? MaxRuntime { get; }
}