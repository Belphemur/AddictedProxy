#region

using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Repositories.Credentials;
using AddictedProxy.Model.Crendentials;
using AddictedProxy.Services.Credentials.Job;
using AddictedProxy.Services.Job.Exception;
using AddictedProxy.Upstream.Service;
using Hangfire;

#endregion

namespace AddictedProxy.Services.Credentials;

public class CredentialsService : ICredentialsService
{
    private readonly IAddictedUserCredentialRepository _addictedUserCredentialRepository;
    private readonly ILogger<CredentialsService> _logger;
    private readonly IAddic7edClient _client;

    public CredentialsService(IAddictedUserCredentialRepository addictedUserCredentialRepository,
                              ILogger<CredentialsService> logger,
                              IAddic7edClient client
    )
    {
        _addictedUserCredentialRepository = addictedUserCredentialRepository;
        _logger = logger;
        _client = client;
    }

    /// <summary>
    /// Get the least used credentials
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If no credentials</exception>
    public async Task<ICredsContainer> GetLeastUsedCredsQueryingAsync(CancellationToken token)
    {
        var creds = await _addictedUserCredentialRepository.GetLeastUsedCredQueryingAsync(token);
        if (creds == null)
        {
            throw new ArgumentNullException("creds", "Was expecting some user creds");
        }

        return new CredsContainerNormalUsage(this, creds, false);
    }

    /// <summary>
    /// Get the least used credentials for download
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If no credentials</exception>
    public async Task<ICredsContainer?> GetLeastUsedCredsDownloadAsync(CancellationToken token)
    {
        var creds = await _addictedUserCredentialRepository.GetLeastUsedCredDownloadAsync(token);
        if (creds == null)
        {
            _logger.LogWarning("Couldn't find any credentials for downloading");
            return null;
        }

        return new CredsContainerNormalUsage(this, creds, true);
    }

    public async Task UpdateUsageCredentialsAsync(AddictedUserCredentials credentials, bool isDownload, CancellationToken token)
    {
        int count;
        string usageType;
        if (isDownload)
        {
            count = ++credentials.DownloadUsage;
            usageType = "download";
        }
        else
        {
            count = ++credentials.Usage;
            usageType = "querying";
        }

        _logger.LogInformation("Update usage of {Cred} to {Count} for {Type}", credentials.Id, count, usageType);

        credentials.LastUsage = DateTime.UtcNow;
        await _addictedUserCredentialRepository.SingleUpdateAsync(credentials, token);
    }

    /// <summary>
    /// Redeem credentials that are currently expired
    /// </summary>
    public async Task RedeemDownloadCredentialsAsync(DateTime currentDateTime, CancellationToken token)
    {
        _logger.LogInformation("[Redeem] Fetching creds to be redeemed for download usage");
        await foreach (var cred in _addictedUserCredentialRepository.GetDownloadExceededCredentialsAsync()
                                                                    .WithCancellation(token))
        {
            BackgroundJob.Enqueue<ResetDownloadCredJob>(job => job.CheckAndResetCredAsync(cred.Id, token));
        }
    }

    /// <summary>
    /// Check on the source (addic7ed website) the quota of the credential. If not busted, reset the credential quota.
    /// </summary>
    /// <param name="credentialId"></param>
    /// <param name="token"></param>
    /// <exception cref="RetryJobException">If something went wrong when parsing the credential from the website</exception>
    public async Task CheckAndResetCredentialsAsync(long credentialId, CancellationToken token)
    {
        var cred = await _addictedUserCredentialRepository.GetCredByIdAsync(credentialId, token);
        if (cred == null)
        {
            _logger.LogError("Couldn't find credential with ID: {id}", credentialId);
            return;
        }

        var downloadCount = await _client.GetDownloadUsageAsync(cred, token);

        try
        {
            cred.DownloadUsage = downloadCount.Used;

            if (downloadCount.FullyUsed)
            {
                _logger.LogInformation("Credentials Id({id}) is fully used for today", credentialId);
                return;
            }

            cred.DownloadExceededDate = null;
        }
        finally
        {
            await _addictedUserCredentialRepository.SingleUpdateAsync(cred, token);
        }
    }
}