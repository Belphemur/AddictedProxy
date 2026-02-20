using AddictedProxy.Database.Model.Shows;
using InversionOfControl.Model.Factory;

namespace AddictedProxy.Services.Provider.Subtitle.Download;

/// <summary>
/// Provider-agnostic subtitle downloader. Each provider implements this to handle
/// its own download mechanism (e.g., Addic7ed uses HTTP with credentials, SuperSubtitles uses gRPC).
/// </summary>
public interface ISubtitleDownloader : IEnumService<DataSource>
{
    /// <summary>
    /// Download a subtitle file from the upstream provider.
    /// </summary>
    /// <param name="subtitle">The subtitle to download.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A stream containing the subtitle file content.</returns>
    Task<Stream> DownloadSubtitleAsync(Database.Model.Shows.Subtitle subtitle, CancellationToken token);
}
