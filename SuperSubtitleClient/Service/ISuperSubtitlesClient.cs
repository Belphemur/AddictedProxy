using Grpc.Core;
using SuperSubtitleClient.Generated;

namespace SuperSubtitleClient.Service;

/// <summary>
/// Client for communicating with the SuperSubtitles gRPC service.
/// </summary>
public interface ISuperSubtitlesClient
{
    /// <summary>
    /// Stream all available TV shows.
    /// </summary>
    IAsyncEnumerable<Show> GetShowListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream all subtitles for a specific show.
    /// </summary>
    IAsyncEnumerable<Subtitle> GetSubtitlesAsync(long showId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream show information and subtitles for multiple shows.
    /// </summary>
    IAsyncEnumerable<ShowSubtitleItem> GetShowSubtitlesAsync(IEnumerable<Show> shows, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if new subtitles are available since a given content ID.
    /// </summary>
    Task<CheckForUpdatesResponse> CheckForUpdatesAsync(string contentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a subtitle file by ID, optionally extracting a specific episode from a season pack.
    /// </summary>
    Task<DownloadSubtitleResponse> DownloadSubtitleAsync(string subtitleId, int? episode = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream recently uploaded subtitles with show information since a given subtitle ID.
    /// </summary>
    IAsyncEnumerable<ShowSubtitleItem> GetRecentSubtitlesAsync(long sinceId, CancellationToken cancellationToken = default);
}
