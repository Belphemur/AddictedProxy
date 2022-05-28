#region

using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Model.Crendentials;

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
    Task<ICredsContainer> GetLeastUsedCredsAsync(CancellationToken token);

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
}