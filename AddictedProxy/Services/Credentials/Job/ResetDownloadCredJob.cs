using AddictedProxy.Database.Repositories.Credentials;
using AddictedProxy.Services.Job.Exception;
using AddictedProxy.Services.Job.Filter;
using AddictedProxy.Upstream.Service;
using Hangfire;

namespace AddictedProxy.Services.Credentials.Job;

public class ResetDownloadCredJob
{
    private readonly IAddic7edClient _client;
    private readonly IAddictedUserCredentialRepository _repository;
    private readonly ILogger<ResetDownloadCredJob> _logger;

    public ResetDownloadCredJob(IAddic7edClient client, IAddictedUserCredentialRepository repository, ILogger<ResetDownloadCredJob> logger)
    {
        _client = client;
        _repository = repository;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 5)]
    [DisableMultipleQueuedItemsFilter]
    [Queue("download-creds-checker")]
    [MaximumConcurrentExecutions(3)]
    public async Task CheckAndResetCredAsync(long credentialId, CancellationToken token)
    {
        var cred = await _repository.GetCredByIdAsync(credentialId, token);
        if (cred == null)
        {
            _logger.LogError("Couldn't find credential with ID: {id}", credentialId);
            return;
        }

        var downloadCount = await _client.GetDownloadUsageAsync(cred, token);
        //Something went wrong, let's reschedule the job
        if (downloadCount == null)
        {
            throw new RetryJobException();
        }

        var downloadUsage = downloadCount.Value;
        if (downloadUsage.FullyUsed)
        {
            _logger.LogInformation("Credentials Id({id}) is fully used for today", credentialId);
            return;
        }

        cred.DownloadUsage = downloadUsage.Used;
        cred.DownloadExceededDate = null;
        await _repository.SingleUpdateAsync(cred, token);
    }
}