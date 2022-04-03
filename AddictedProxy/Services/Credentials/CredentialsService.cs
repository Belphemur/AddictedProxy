#region

using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Repositories.Credentials;
using AddictedProxy.Model.Crendentials;

#endregion

namespace AddictedProxy.Services.Credentials;

public class CredentialsService : ICredentialsService
{
    private readonly IAddictedUserCredentialRepository _addictedUserCredentialRepository;

    public CredentialsService(IAddictedUserCredentialRepository addictedUserCredentialRepository)
    {
        _addictedUserCredentialRepository = addictedUserCredentialRepository;
    }

    /// <summary>
    /// Get the least used credentials
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If no credentials</exception>
    public async Task<CredsContainer> GetLeastUsedCredsAsync(CancellationToken token)
    {
        var creds = await _addictedUserCredentialRepository.GetLeastUsedCredAsync(token);
        if (creds == null)
        {
            throw new ArgumentNullException("creds", "Was expecting some user creds");
        }

        return new(this, creds);
    }

    public async Task UpdateUsageCredentialsAsync(AddictedUserCredentials credentials, CancellationToken token)
    {
        credentials.Usage++;
        credentials.LastUsage = DateTime.UtcNow;
        await _addictedUserCredentialRepository.UpsertUserCredentialsAsync(credentials, token);
    }
}