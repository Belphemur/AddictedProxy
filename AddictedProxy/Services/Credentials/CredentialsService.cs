#region

using System.Runtime.CompilerServices;
using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Repositories.Credentials;
using AddictedProxy.Database.Transaction;
using AddictedProxy.Model.Crendentials;
using AddictedProxy.Services.Provider.Config;
using Microsoft.Extensions.Options;

#endregion

namespace AddictedProxy.Services.Credentials;

public class CredentialsService : ICredentialsService
{
    private readonly IAddictedUserCredentialRepository _addictedUserCredentialRepository;
    private readonly IOptions<RefreshConfig> _refreshConfig;
    private readonly ILogger<CredentialsService> _logger;

    public CredentialsService(IAddictedUserCredentialRepository addictedUserCredentialRepository,
        IOptions<RefreshConfig> refreshConfig,
        ILogger<CredentialsService> logger
    )
    {
        _addictedUserCredentialRepository = addictedUserCredentialRepository;
        _refreshConfig = refreshConfig;
        _logger = logger;
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
        await _addictedUserCredentialRepository.SaveChangesAsync(token);
    }

    /// <summary>
    /// Redeem credentials that are currently expired
    /// </summary>
    public async Task RedeemDownloadCredentialsAsync(DateTime currentDateTime, CancellationToken token)
    {
        var hasRedeemedCreds = false;
        _logger.LogInformation("[Redeem] Fetching creds to be redeemed for download usage");
        var ids = new List<long>();
        await foreach (var cred in _addictedUserCredentialRepository.GetDownloadExceededCredentialsAsync().Where(cred => currentDateTime - cred.DownloadExceededDate >= _refreshConfig.Value.DownloadExceededTimeout).WithCancellation(token))
        {
            cred.DownloadExceededDate = null;
            cred.DownloadUsage = 0;
            hasRedeemedCreds = true;
            ids.Add(cred.Id);
        }
        _logger.LogInformation("[Redeem] Redeemed those creds: [{Ids}]", string.Join(", ", ids));

        if (!hasRedeemedCreds)
        {
            return;
        }

        await _addictedUserCredentialRepository.SaveChangesAsync(token);
    }
}