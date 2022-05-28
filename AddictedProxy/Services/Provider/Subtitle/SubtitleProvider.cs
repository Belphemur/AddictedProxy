#region

using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Provider.Subtitle.Jobs;
using AddictedProxy.Storage.Store;
using AddictedProxy.Upstream.Service;
using AddictedProxy.Upstream.Service.Exception;
using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.Scheduler;

#endregion

namespace AddictedProxy.Services.Provider.Subtitle;

public class SubtitleProvider : ISubtitleProvider
{
    private readonly IAddic7edDownloader _addic7EdDownloader;
    private readonly ICredentialsService _credentialsService;
    private readonly IJobBuilder _jobBuilder;
    private readonly IJobScheduler _jobScheduler;
    private readonly IStorageProvider _storageProvider;
    private readonly ISubtitleRepository _subtitleRepository;

    public SubtitleProvider(IAddic7edDownloader addic7EdDownloader,
                            IStorageProvider storageProvider,
                            ISubtitleRepository subtitleRepository,
                            ICredentialsService credentialsService,
                            IJobBuilder jobBuilder,
                            IJobScheduler jobScheduler)
    {
        _addic7EdDownloader = addic7EdDownloader;
        _storageProvider = storageProvider;
        _subtitleRepository = subtitleRepository;
        _credentialsService = credentialsService;
        _jobBuilder = jobBuilder;
        _jobScheduler = jobScheduler;
    }

    private class SubtitleCounterUpdater : IAsyncDisposable
    {
        private readonly ISubtitleRepository _subtitleRepository;
        private readonly Database.Model.Shows.Subtitle _subtitle;

        public SubtitleCounterUpdater(ISubtitleRepository subtitleRepository, Database.Model.Shows.Subtitle subtitle)
        {
            _subtitleRepository = subtitleRepository;
            _subtitle = subtitle;
        }

        public async ValueTask DisposeAsync()
        {
            _subtitle.DownloadCount++;
            await _subtitleRepository.SaveChangeAsync(CancellationToken.None);
        }
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
        await using var subDownloadUpdater = new SubtitleCounterUpdater(_subtitleRepository, subtitle);
        //We have the subtitle stored
        if (subtitle.StoragePath != null)
        {
            var stream = await _storageProvider.DownloadAsync(subtitle.StoragePath, token);
            if (stream != null)
            {
                return stream;
            }
        }

        await using var creds = await _credentialsService.GetLeastUsedCredsDownloadAsync(token);
        try
        {
            //Subtitle isn't complete, no need to store it, just directly return the download stream
            if (!subtitle.Completed)
            {
                return await _addic7EdDownloader.DownloadSubtitle(creds?.AddictedUserCredentials, subtitle, token);
            }


            await using var subtitleStream = await _addic7EdDownloader.DownloadSubtitle(creds?.AddictedUserCredentials, subtitle, token);
            await using var buffer = new MemoryStream();

            await subtitleStream.CopyToAsync(buffer, token);

            var blob = buffer.ToArray();

            _jobScheduler.ScheduleJob(
                _jobBuilder.Create<StoreSubtitleJob>()
                           .Configure(job =>
                           {
                               job.SubtitleBlob = blob;
                               job.SubtitleId = subtitle.UniqueId;
                           })
                           .Build()
            );
            return new MemoryStream(blob);
        }
        catch (DownloadLimitExceededException)
        {
            creds.TagAsDownloadExceeded();
            throw;
        }
    }

    public Task<Database.Model.Shows.Subtitle?> GetSubtitleFullAsync(Guid subtitleId, CancellationToken token)
    {
        return _subtitleRepository.GetSubtitleByGuidAsync(subtitleId, true, true, token);
    }
}