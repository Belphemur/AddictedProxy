using AddictedProxy.Database.Repositories.Credentials;
using AddictedProxy.OneTimeMigration.Model;
using AddictedProxy.Upstream.Service;

namespace AddictedProxy.Migrations.Services;

public class CleanUpInboxUsersMigration : IMigration
{
    private readonly IAddic7edClient _addic7EdClient;
    private readonly IAddictedUserCredentialRepository _credentialRepository;

    public CleanUpInboxUsersMigration(IAddic7edClient addic7EdClient, IAddictedUserCredentialRepository credentialRepository)
    {
        _addic7EdClient = addic7EdClient;
        _credentialRepository = credentialRepository;
    }

    public async Task ExecuteAsync(CancellationToken token)
    {
        var creds = await _credentialRepository.GetAllCredentialsAsync().ToArrayAsync(token);
        foreach (var cred in creds)
        {
            await _addic7EdClient.CleanupInbox(cred, token);
            cred.DownloadExceededDate = null;
            cred.Usage = 0;
            cred.DownloadUsage = 0;
        }

        await _credentialRepository.BulkUpdateAsync(creds, token);
    }
}