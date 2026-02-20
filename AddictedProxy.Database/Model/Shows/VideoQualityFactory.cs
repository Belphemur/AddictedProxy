using System.Collections.Frozen;

namespace AddictedProxy.Database.Model.Shows;

/// <summary>
/// Factory for converting <see cref="VideoQuality"/> flags to human-readable display names.
/// </summary>
public static class VideoQualityFactory
{
    private static readonly FrozenDictionary<VideoQuality, string> DisplayNames = new Dictionary<VideoQuality, string>
    {
        [VideoQuality.Q360P] = "360p",
        [VideoQuality.Q480P] = "480p",
        [VideoQuality.Q720P] = "720p",
        [VideoQuality.Q1080P] = "1080p",
        [VideoQuality.Q2160P] = "4K",
    }.ToFrozenDictionary();

    /// <summary>
    /// Get the display name for a single <see cref="VideoQuality"/> value.
    /// </summary>
    public static string GetDisplayName(VideoQuality quality) =>
        DisplayNames.GetValueOrDefault(quality, quality.ToString());

    /// <summary>
    /// Decompose a <see cref="VideoQuality"/> bitmask into an array of display name strings.
    /// </summary>
    public static string[] ToDisplayNames(VideoQuality qualities)
    {
        if (qualities == VideoQuality.None)
            return [];

        return Enum.GetValues<VideoQuality>()
            .Where(q => q != VideoQuality.None && qualities.HasFlag(q))
            .Select(GetDisplayName)
            .ToArray();
    }
}
