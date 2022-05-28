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
    public async Task<ICredsContainer> GetLeastUsedCredsDownloadAsync(CancellationToken token)
    {
        var creds = await _addictedUserCredentialRepository.GetLeastUsedCredDownloadAsync(token);
        if (creds == null)
        {
            throw new ArgumentNullException("creds", "Was expecting some user creds");
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
}