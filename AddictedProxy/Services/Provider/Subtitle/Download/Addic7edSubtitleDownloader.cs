using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Upstream.Service;
using AddictedProxy.Upstream.Service.Exception;

namespace AddictedProxy.Services.Provider.Subtitle.Download;

/// <summary>
/// Downloads subtitles from Addic7ed using the existing credential rotation mechanism.
/// Handles <see cref="DownloadLimitExceededException"/> by tagging credentials as exceeded and retrying with a new set.
/// </summary>
internal class Addic7edSubtitleDownloader : ISubtitleDownloader
{
    private readonly IAddic7edDownloader _addic7EdDownloader;
    private readonly ICredentialsService _credentialsService;
    private readonly ILogger<Addic7edSubtitleDownloader> _logger;
    private const int MaxAttempts = 3;

    public Addic7edSubtitleDownloader(IAddic7edDownloader addic7EdDownloader,
                                      ICredentialsService credentialsService,
                                      ILogger<Addic7edSubtitleDownloader> logger)
    {
        _addic7EdDownloader = addic7EdDownloader;
        _credentialsService = credentialsService;
        _logger = logger;
    }

    public DataSource Enum => DataSource.Addic7ed;

    /// <inheritdoc />
    public Task<Stream> DownloadSubtitleAsync(Database.Model.Shows.Subtitle subtitle, CancellationToken token)
    {
        return DownloadWithRetryAsync(subtitle, 0, token);
    }

    private async Task<Stream> DownloadWithRetryAsync(Database.Model.Shows.Subtitle subtitle, int attempts, CancellationToken token)
    {
        if (attempts >= MaxAttempts)
        {
            throw new DownloadLimitExceededException($"Reached maximum attempts ({MaxAttempts}) to download subtitle from Addic7ed");
        }

        await using (var creds = await _credentialsService.GetLeastUsedCredsDownloadAsync(token))
        {
            try
            {
                return await _addic7EdDownloader.DownloadSubtitle(creds?.AddictedUserCredentials, subtitle, token);
            }
            catch (DownloadLimitExceededException)
            {
                _logger.LogWarning("Download limit exceeded for credential {CredentialId}, attempt {Attempt}/{MaxAttempts}",
                    creds?.AddictedUserCredentials?.Id, attempts + 1, MaxAttempts);
                creds?.TagAsDownloadExceeded();
            }
        }

        await Task.Delay(TimeSpan.FromMilliseconds(100), token);
        return await DownloadWithRetryAsync(subtitle, attempts + 1, token);
    }
}
