using AddictedProxy.Database.Repositories.Credentials;
using AddictedProxy.OneTimeMigration.Model;
using AddictedProxy.Upstream.Service;

namespace AddictedProxy.Migrations.Services;

[MigrationDate(2024, 08, 14)]
public class CleanUpInboxUsersMigration : IMigration
{
    private readonly IAddic7edClient _addic7EdClient;
    private readonly IAddictedUserCredentialRepository _credentialRepository;
    private readonly ILogger<CleanUpInboxUsersMigration> _logger;

    public CleanUpInboxUsersMigration(IAddic7edClient addic7EdClient, IAddictedUserCredentialRepository credentialRepository, ILogger<CleanUpInboxUsersMigration> logger)
    {
        _addic7EdClient = addic7EdClient;
        _credentialRepository = credentialRepository;
        _logger = logger;
    }

    public async Task ExecuteAsync(Hangfire.Server.PerformContext context, CancellationToken token)
    {
        var creds = await _credentialRepository.GetAllCredentialsAsync().ToArrayAsync(token);
        foreach (var cred in creds)
        {
            if (!await _addic7EdClient.CleanupInbox(cred, token))
            {
                _logger.LogWarning("Couldn't clean up inbox for {CredentialId}", cred.Id);
            }

            cred.DownloadExceededDate = null;
            cred.Usage = 0;
            cred.DownloadUsage = 0;
        }

        await _credentialRepository.BulkUpdateAsync(creds, token);
    }
}