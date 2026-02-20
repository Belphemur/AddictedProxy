#region

using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Provider.Subtitle.Download;
using AddictedProxy.Services.Provider.Subtitle.Jobs;
using AddictedProxy.Storage.Caching.Service;
using AddictedProxy.Upstream.Service.Exception;
using Hangfire;

#endregion

namespace AddictedProxy.Services.Provider.Subtitle;

internal class SubtitleProvider : ISubtitleProvider
{
    private readonly SubtitleDownloaderFactory _downloaderFactory;
    private readonly ILogger<SubtitleProvider> _logger;
    private readonly SubtitleCounterUpdater _subtitleCounterUpdater;
    private readonly ICachedStorageProvider _cachedStorageProvider;
    private readonly ISubtitleRepository _subtitleRepository;

    public SubtitleProvider(SubtitleDownloaderFactory downloaderFactory,
                            ICachedStorageProvider cachedStorageProvider,
                            ISubtitleRepository subtitleRepository,
                            ILogger<SubtitleProvider> logger,
                            SubtitleCounterUpdater subtitleCounterUpdater)
    {
        _downloaderFactory = downloaderFactory;
        _cachedStorageProvider = cachedStorageProvider;
        _subtitleRepository = subtitleRepository;
        _logger = logger;
        _subtitleCounterUpdater = subtitleCounterUpdater;
    }

    /// <summary>
    /// Get the subtitle file stream
    /// </summary>
    /// <param name="subtitle"></param>
    /// <param name="token"></param>
    /// <exception cref="DownloadLimitExceededException">When we reach limit in Addicted to download the subtitle</exception>
    /// <returns></returns>
    public async Task<Stream> GetSubtitleFileAsync(Database.Model.Shows.Subtitle subtitle, CancellationToken token)
    {
        //We have the subtitle stored
        if (subtitle.StoragePath != null)
        {
            var stream = await _cachedStorageProvider.GetSertAsync("subtitle", subtitle.StoragePath, token);
            if (stream != null)
            {
                await _subtitleCounterUpdater.IncrementSubtitleCountAsync(subtitle, token);
                return stream;
            }

            _logger.LogWarning("Couldn't find subtitle with path [{path}] in storage, even if we have a path for it", subtitle.StoragePath);
        }

        return await DownloadStoreSubtitleAsync(subtitle, token);
    }

    private async Task<Stream> DownloadStoreSubtitleAsync(Database.Model.Shows.Subtitle subtitle, CancellationToken token)
    {
        var downloader = _downloaderFactory.GetService(subtitle.Source);

        try
        {
            //Subtitle isn't complete, no need to store it, just directly return the download stream
            if (!subtitle.Completed)
            {
                await _subtitleCounterUpdater.IncrementSubtitleCountAsync(subtitle, token);
                return await downloader.DownloadSubtitleAsync(subtitle, token);
            }

            await using var subtitleStream = await downloader.DownloadSubtitleAsync(subtitle, token);
            await using var buffer = new MemoryStream();

            await subtitleStream.CopyToAsync(buffer, token);

            var blob = buffer.ToArray();

            BackgroundJob.Enqueue<StoreSubtitleJob>(job => job.ExecuteAsync(subtitle.UniqueId, blob, null, default));

            await _subtitleCounterUpdater.IncrementSubtitleCountAsync(subtitle, token);
            return new MemoryStream(blob);
        }
        catch (SubtitleFileDeletedException)
        {
            _subtitleRepository.TagForRemoval(subtitle);
            await _subtitleRepository.SaveChangeAsync(token);
            throw;
        }
    }

    public Task<Database.Model.Shows.Subtitle?> GetSubtitleFullAsync(Guid subtitleId, CancellationToken token)
    {
        return _subtitleRepository.GetSubtitleByGuidAsync(subtitleId, true, true, token);
    }
}