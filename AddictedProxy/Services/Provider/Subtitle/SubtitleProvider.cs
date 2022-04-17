#region

using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Provider.Subtitle.Job;
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

    /// <summary>
    /// Get the subtitle file stream
    /// </summary>
    /// <param name="subtitle"></param>
    /// <param name="token"></param>
    /// <exception cref="DownloadLimitExceededException">When we reach limit in Addicted to download the subtitle</exception>
    /// <returns></returns>
    public async Task<Stream> GetSubtitleFileAsync(Database.Model.Shows.Subtitle subtitle, CancellationToken token)
    {
        if (subtitle.StoragePath != null)
        {
            return await _storageProvider.DownloadAsync(subtitle.StoragePath, token);
        }

        await using var creds = await _credentialsService.GetLeastUsedCredsAsync(token);

        await using var subtitleStream = await _addic7EdDownloader.DownloadSubtitle(creds.AddictedUserCredentials, subtitle, token);
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

    public Task<Database.Model.Shows.Subtitle?> GetSubtitleAsync(Guid subtitleId, CancellationToken token)
    {
        return _subtitleRepository.GetSubtitleByGuidAsync(subtitleId, false, token);
    }
}