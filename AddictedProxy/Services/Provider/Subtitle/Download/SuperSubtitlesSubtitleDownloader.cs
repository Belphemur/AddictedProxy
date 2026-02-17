using AddictedProxy.Database.Model.Shows;
using SuperSubtitleClient.Service;

namespace AddictedProxy.Services.Provider.Subtitle.Download;

/// <summary>
/// Downloads subtitles from SuperSubtitles using the gRPC <c>DownloadSubtitle</c> method.
/// No credentials or rate limiting needed — the SuperSubtitles server handles upstream communication.
/// </summary>
/// <remarks>
/// SuperSubtitles subtitles store their upstream subtitle ID in <see cref="Database.Model.Shows.Subtitle.ExternalId"/>.
/// </remarks>
internal class SuperSubtitlesSubtitleDownloader : ISubtitleDownloader
{
    private readonly ISuperSubtitlesClient _superSubtitlesClient;

    public SuperSubtitlesSubtitleDownloader(ISuperSubtitlesClient superSubtitlesClient)
    {
        _superSubtitlesClient = superSubtitlesClient;
    }

    public DataSource Enum => DataSource.SuperSubtitles;

    /// <inheritdoc />
    public async Task<Stream> DownloadSubtitleAsync(Database.Model.Shows.Subtitle subtitle, CancellationToken token)
    {
        if (string.IsNullOrEmpty(subtitle.ExternalId))
        {
            throw new InvalidOperationException(
                $"Subtitle {subtitle.Id} (Source={subtitle.Source}) has no ExternalId set");
        }

        var response = await _superSubtitlesClient.DownloadSubtitleAsync(subtitle.ExternalId, episode: null, cancellationToken: token);
        return new MemoryStream(response.Content.ToByteArray());
    }
}
