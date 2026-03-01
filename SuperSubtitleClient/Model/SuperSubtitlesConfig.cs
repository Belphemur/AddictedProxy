namespace SuperSubtitleClient.Model;

/// <summary>
/// Configuration for the SuperSubtitles gRPC client.
/// Bound from the "SuperSubtitles" configuration section.
/// </summary>
public class SuperSubtitlesConfig
{
    public const string SectionName = "SuperSubtitles";

    /// <summary>
    /// The gRPC endpoint address of the SuperSubtitles service (e.g., "http://localhost:3001").
    /// </summary>
    public required Uri Address { get; init; }

    /// <summary>
    /// How long a pooled gRPC connection may be reused before it is recycled.
    /// Rotating connections periodically prevents stale connections after multi-day uptime.
    /// Defaults to 6 hours.
    /// </summary>
    public TimeSpan ConnectionLifetime { get; init; } = TimeSpan.FromHours(6);
}
