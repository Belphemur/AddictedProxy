#region

using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Provider.Subtitle.Jobs;
using AddictedProxy.Storage.Store;
using AddictedProxy.Upstream.Service;
using AddictedProxy.Upstream.Service.Exception;
using Hangfire;

#endregion

namespace AddictedProxy.Services.Provider.Subtitle;

internal class SubtitleProvider : ISubtitleProvider
{
    private readonly IAddic7edDownloader _addic7EdDownloader;
    private readonly ICredentialsService _credentialsService;
    private readonly ILogger<SubtitleProvider> _logger;
    private readonly SubtitleCounterUpdater _subtitleCounterUpdater;
    private readonly IStorageProvider _storageProvider;
    private readonly ISubtitleRepository _subtitleRepository;
    private const int MAX_ATTEMPTS = 3;

    public SubtitleProvider(IAddic7edDownloader addic7EdDownloader,
                            IStorageProvider storageProvider,
                            ISubtitleRepository subtitleRepository,
                            ICredentialsService credentialsService,
                            ILogger<SubtitleProvider> logger,
                            SubtitleCounterUpdater subtitleCounterUpdater)
    {
        _addic7EdDownloader = addic7EdDownloader;
        _storageProvider = storageProvider;
        _subtitleRepository = subtitleRepository;
        _credentialsService = credentialsService;
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
            var stream = await _storageProvider.DownloadAsync(subtitle.StoragePath, token);
            if (stream != null)
            {
                await _subtitleCounterUpdater.IncrementSubtitleCountAsync(subtitle, token);
                return stream;
            }

            _logger.LogWarning("Couldn't find subtitle with path [{path}] in storage, even if we have a path for it", subtitle.StoragePath);
        }

        return await DownloadStoreSubtitleAsync(subtitle, 0, token);
    }

    private async Task<Stream> DownloadStoreSubtitleAsync(Database.Model.Shows.Subtitle subtitle, int attempts, CancellationToken token)
    {
        if (attempts >= MAX_ATTEMPTS)
        {
            throw new DownloadLimitExceededException($"Reached maximum attempts ({MAX_ATTEMPTS}) to download subtitle");
        }

        await using (var creds = await _credentialsService.GetLeastUsedCredsDownloadAsync(token))
        {
            try
            {
                //Subtitle isn't complete, no need to store it, just directly return the download stream
                if (!subtitle.Completed)
                {
                    await _subtitleCounterUpdater.IncrementSubtitleCountAsync(subtitle, token);
                    return await _addic7EdDownloader.DownloadSubtitle(creds?.AddictedUserCredentials, subtitle, token);
                }


                await using var subtitleStream = await _addic7EdDownloader.DownloadSubtitle(creds?.AddictedUserCredentials, subtitle, token);
                await using var buffer = new MemoryStream();

                await subtitleStream.CopyToAsync(buffer, token);

                var blob = buffer.ToArray();

                BackgroundJob.Enqueue<StoreSubtitleJob>(job => job.ExecuteAsync(subtitle.UniqueId, blob, default));


                await _subtitleCounterUpdater.IncrementSubtitleCountAsync(subtitle, token);
                return new MemoryStream(blob);
            }
            catch (DownloadLimitExceededException)
            {
                creds?.TagAsDownloadExceeded();
            }
        }

        await Task.Delay(TimeSpan.FromMilliseconds(100), token);
        return await DownloadStoreSubtitleAsync(subtitle, attempts + 1, token);
    }

    public Task<Database.Model.Shows.Subtitle?> GetSubtitleFullAsync(Guid subtitleId, CancellationToken token)
    {
        return _subtitleRepository.GetSubtitleByGuidAsync(subtitleId, true, true, token);
    }
}