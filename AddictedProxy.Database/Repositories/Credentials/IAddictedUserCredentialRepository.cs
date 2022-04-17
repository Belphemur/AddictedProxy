#region

using AddictedProxy.Database.Model.Credentials;

#endregion

namespace AddictedProxy.Database.Repositories.Credentials;

public interface IAddictedUserCredentialRepository
{
    /// <summary>
    /// Get the least used creds
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<AddictedUserCredentials?> GetLeastUsedCredAsync(CancellationToken token);

    /// <summary>
    /// Update the data of the creds
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task UpsertUserCredentialsAsync(AddictedUserCredentials credentials, CancellationToken token);
}