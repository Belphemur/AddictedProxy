using Grpc.Core;

namespace SuperSubtitleClient.Service.Exception;

/// <summary>
/// Thrown when a subtitle download from SuperSubtitles fails with a gRPC error.
/// </summary>
public class SuperSubtitleDownloadException : System.Exception
{
    public SuperSubtitleDownloadException(string subtitleId, RpcException inner)
        : base($"Failed to download subtitle '{subtitleId}' from SuperSubtitles: {inner.Status.Detail}", inner)
    {
    }
}
