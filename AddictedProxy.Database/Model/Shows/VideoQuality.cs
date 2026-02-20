namespace AddictedProxy.Database.Model.Shows;

/// <summary>
/// Bitmask flags representing the video quality of a subtitle.
/// Addic7ed only exposes a boolean HD flag; SuperSubtitles provides exact resolution.
/// </summary>
[Flags]
public enum VideoQuality
{
    None   = 0,
    Q360P  = 1 << 0,  // 1
    Q480P  = 1 << 1,  // 2
    Q720P  = 1 << 2,  // 4
    Q1080P = 1 << 3,  // 8
    Q2160P = 1 << 4,  // 16
}
