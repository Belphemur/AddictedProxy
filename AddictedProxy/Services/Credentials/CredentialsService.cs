#region

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
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<CredentialsService> _logger;

    public CredentialsService(IAddictedUserCredentialRepository addictedUserCredentialRepository,
                              IOptions<RefreshConfig> refreshConfig,
                              ITransactionManager transactionManager,
                              ILogger<CredentialsService> logger)
    {
        _addictedUserCredentialRepository = addictedUserCredentialRepository;
        _refreshConfig = refreshConfig;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    /// <summary>
    /// Get the least used credentials
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If no credentials</exception>
    public async Task<ICredsContainer> GetLeastUsedCredsAsync(CancellationToken token)
    {
        var creds = await _addictedUserCredentialRepository.GetLeastUsedCredAsync(token);
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
  
        if (isDownload)
        {
            credentials.DownloadUsage++;
        }
        else
        {
            credentials.Usage++;
        }

        credentials.LastUsage = DateTime.UtcNow;
        await _addictedUserCredentialRepository.SaveChangesAsync(token);
    }

    /// <summary>
    /// Redeem credentials that are currently expired
    /// </summary>
    public async Task RedeemDownloadCredentialsAsync(DateTime currentDateTime, CancellationToken token)
    {
        await using var transaction = await _transactionManager.BeginNestedAsync(token);
        var credentials = await _addictedUserCredentialRepository.GetAllCredentialsAsync().ToArrayAsync(token);
        var hasRedeemedCreds = false;
        foreach (var cred in credentials.Where(cred => cred.DownloadExceededDate != null && currentDateTime - cred.DownloadExceededDate >= _refreshConfig.Value.DownloadExceededTimeout))
        {
            cred.DownloadExceededDate = null;
            hasRedeemedCreds = true;
        }

        if (!hasRedeemedCreds)
        {
            return;
        }

        foreach (var cred in credentials)
        {
            cred.DownloadUsage = 0;
        }

        await _addictedUserCredentialRepository.SaveChangesAsync(token);
        await transaction.CommitAsync(token);
    }
}