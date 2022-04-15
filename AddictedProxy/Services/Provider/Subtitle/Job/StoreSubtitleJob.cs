using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Storage.Store;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

namespace AddictedProxy.Services.Provider.Subtitle.Job;

public class StoreSubtitleJob : IJob
{
    private readonly ILogger<StoreSubtitleJob> _logger;
    private readonly Guid _subtitleId;
    private readonly byte[] _subtitleBlob;
    private readonly IServiceProvider _serviceProvider;
    private readonly IStorageProvider _storageProvider;

    public StoreSubtitleJob(IServiceProvider serviceProvider, Guid subtitleId, byte[] subtitleBlob)
    {
        _serviceProvider = serviceProvider;
        _storageProvider = _serviceProvider.GetRequiredService<IStorageProvider>();
        _logger = _serviceProvider.GetRequiredService<ILogger<StoreSubtitleJob>>();
        _subtitleId = subtitleId;
        _subtitleBlob = subtitleBlob;
    }

    private string GetStorageName(Database.Model.Shows.Subtitle subtitle)
        => $"{subtitle.Episode.TvShowId}/{subtitle.Episode.Season}/{subtitle.Episode.Number}/{subtitle.UniqueId}.srt";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Saving subtitle {subtitleId} into the storage", _subtitleId);
        await using var scope = _serviceProvider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISubtitleRepository>();
        var subtitle = await repository.GetSubtitleByGuidAsync(_subtitleId, true, cancellationToken);
        if (subtitle == null)
        {
            _logger.LogWarning("Subtitle couldn't be found with GUID {subtitleId}", _subtitleId);
            return;
        }

        await using var buffer = new MemoryStream(_subtitleBlob);
        var storageName = GetStorageName(subtitle);
        if (!await _storageProvider.StoreAsync(storageName, buffer, cancellationToken))
        {
            throw new InvalidOperationException($"Couldn't store the subtitle {_subtitleId}");
        }

        subtitle.StoragePath = storageName;
        await repository.UpdateAsync(subtitle, cancellationToken);
    }

    public Task OnFailure(JobException exception)
    {
        _logger.LogError(exception, "Issue while saving subtitle {subtitleId} in storage", _subtitleId);
        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new ExponentialBackoffRetry(TimeSpan.FromSeconds(1), 10);
    public TimeSpan? MaxRuntime { get; }
}