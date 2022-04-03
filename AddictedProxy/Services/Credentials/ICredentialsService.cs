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
    /// <param name="token"></param>
    Task UpdateUsageCredentialsAsync(AddictedUserCredentials credentials, CancellationToken token);

    /// <summary>
    /// Get the least used credentials
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If no crendentials</exception>
    Task<CredsContainer> GetLeastUsedCredsAsync(CancellationToken token);
}