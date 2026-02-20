using System.Runtime.CompilerServices;
using Grpc.Core;
using SuperSubtitleClient.Generated;

namespace SuperSubtitleClient.Service;

/// <summary>
/// Wrapper around the generated gRPC client for SuperSubtitles.
/// Translates server-side streaming calls into <see cref="IAsyncEnumerable{T}"/>.
/// </summary>
public class SuperSubtitlesClientImpl : ISuperSubtitlesClient
{
    private readonly SuperSubtitlesService.SuperSubtitlesServiceClient _client;

    public SuperSubtitlesClientImpl(SuperSubtitlesService.SuperSubtitlesServiceClient client)
    {
        _client = client;
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Show> GetShowListAsync(CancellationToken cancellationToken = default)
    {
        var call = _client.GetShowList(new GetShowListRequest(), cancellationToken: cancellationToken);
        return ReadStreamAsync(call.ResponseStream, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Subtitle> GetSubtitlesAsync(long showId, CancellationToken cancellationToken = default)
    {
        var call = _client.GetSubtitles(new GetSubtitlesRequest { ShowId = showId }, cancellationToken: cancellationToken);
        return ReadStreamAsync(call.ResponseStream, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<ShowSubtitleItem> GetShowSubtitlesAsync(IEnumerable<Show> shows, CancellationToken cancellationToken = default)
    {
        var request = new GetShowSubtitlesRequest();
        request.Shows.AddRange(shows);
        var call = _client.GetShowSubtitles(request, cancellationToken: cancellationToken);
        return ReadStreamAsync(call.ResponseStream, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CheckForUpdatesResponse> CheckForUpdatesAsync(long contentId, CancellationToken cancellationToken = default)
    {
        return await _client.CheckForUpdatesAsync(new CheckForUpdatesRequest { ContentId = contentId }, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DownloadSubtitleResponse> DownloadSubtitleAsync(string subtitleId, int? episode = null, CancellationToken cancellationToken = default)
    {
        var request = new DownloadSubtitleRequest { SubtitleId = subtitleId };
        if (episode.HasValue)
        {
            request.Episode = episode.Value;
        }

        return await _client.DownloadSubtitleAsync(request, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<ShowSubtitleItem> GetRecentSubtitlesAsync(long sinceId, CancellationToken cancellationToken = default)
    {
        var call = _client.GetRecentSubtitles(new GetRecentSubtitlesRequest { SinceId = sinceId }, cancellationToken: cancellationToken);
        return ReadStreamAsync(call.ResponseStream, cancellationToken);
    }

    private static async IAsyncEnumerable<T> ReadStreamAsync<T>(IAsyncStreamReader<T> stream, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (await stream.MoveNext(cancellationToken))
        {
            yield return stream.Current;
        }
    }
}
