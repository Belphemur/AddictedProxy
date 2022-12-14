#region

using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Model.Crendentials;
using AddictedProxy.Services.Job.Exception;

#endregion

namespace AddictedProxy.Services.Credentials;

public interface ICredentialsService
{
    /// <summary>
    /// Update usage information about the credentials
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="isDownload"></param>
    /// <param name="token"></param>
    public Task UpdateUsageCredentialsAsync(AddictedUserCredentials credentials, bool isDownload, CancellationToken token);

    /// <summary>
    /// Get the least used credentials
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If no crendentials</exception>
    Task<ICredsContainer> GetLeastUsedCredsQueryingAsync(CancellationToken token);

    /// <summary>
    /// Get the least used credentials for download
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If no credentials</exception>
    Task<ICredsContainer?> GetLeastUsedCredsDownloadAsync(CancellationToken token);

    /// <summary>
    /// Redeem credentials that are currently expired
    /// </summary>
    Task RedeemDownloadCredentialsAsync(DateTime currentDateTime, CancellationToken token);

    /// <summary>
    /// Check on the source (addic7ed website) the quota of the credential. If not busted, reset the credential quota.
    /// </summary>
    /// <param name="credentialId"></param>
    /// <param name="token"></param>
    /// <exception cref="RetryJobException">If something went wrong when parsing the credential from the website</exception>
    Task CheckAndResetCredentialsAsync(long credentialId, CancellationToken token);
}