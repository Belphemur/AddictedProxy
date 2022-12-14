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
    Task<AddictedUserCredentials?> GetLeastUsedCredQueryingAsync(CancellationToken token);

    /// <summary>
    /// Update the data of the creds
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task SingleUpdateAsync(AddictedUserCredentials credentials, CancellationToken token);


    /// <summary>
    /// Get the least used credential for download
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<AddictedUserCredentials?> GetLeastUsedCredDownloadAsync(CancellationToken token);

    /// <summary>
    /// Get all the credentials
    /// </summary>
    IAsyncEnumerable<AddictedUserCredentials> GetAllCredentialsAsync();

    /// <summary>
    /// Get credentials that have reach their download limits
    /// </summary>
    /// <returns></returns>
    IAsyncEnumerable<AddictedUserCredentials> GetDownloadExceededCredentialsAsync();

    /// <summary>
    /// Bulk update multiple creds
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task BulkUpdateAsync(IEnumerable<AddictedUserCredentials> credentials, CancellationToken token);

    /// <summary>
    /// Get a credential by its id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<AddictedUserCredentials?> GetCredByIdAsync(long id, CancellationToken token);
}