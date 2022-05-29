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
        var hasRedeemedCreds = false;
        await foreach (var cred in _addictedUserCredentialRepository.GetDownloadExceededCredentialsAsync().Where(cred => currentDateTime - cred.DownloadExceededDate >= _refreshConfig.Value.DownloadExceededTimeout).WithCancellation(token))
        {
            cred.DownloadExceededDate = null;
            cred.DownloadUsage = 0;
            hasRedeemedCreds = true;
        }

        if (!hasRedeemedCreds)
        {
            return;
        }


        await _addictedUserCredentialRepository.SaveChangesAsync(token);
        await transaction.CommitAsync(token);
    }
}