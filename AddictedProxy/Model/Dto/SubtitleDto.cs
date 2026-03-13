using System.ComponentModel.DataAnnotations;
using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Model.Dto;

public class SubtitleDto
{
    public SubtitleDto(Subtitle subtitle, string downloadUri, Culture.Model.Culture? language)
    {
        Version = subtitle.Scene;
        Completed = subtitle.Completed;
        HearingImpaired = subtitle.HearingImpaired;
        Corrected = subtitle.Completed;
        DownloadUri = downloadUri;
        Language = language?.EnglishName ?? "Unknown";
        Discovered = subtitle.Discovered;
        SubtitleId = subtitle.UniqueId.ToString();
        DownloadCount = subtitle.DownloadCount;
        Source = subtitle.Source.ToString();
        Qualities = VideoQualityFactory.ToDisplayNames(subtitle.Qualities);
        HD = (subtitle.Qualities & (VideoQuality.Q720P | VideoQuality.Q1080P | VideoQuality.Q2160P)) != VideoQuality.None;
        Release = subtitle.Release;
    }

    /// <summary>
    /// Constructor for season pack fallback entries used in the /subtitles/get/ endpoint when the pack is not yet cataloged.
    /// Maps a season pack to a SubtitleDto shape so Bazarr picks it up without code changes.
    /// </summary>
    public SubtitleDto(SeasonPackSubtitle seasonPack, string downloadUri, Culture.Model.Culture? language, int episode)
    {
        SubtitleId = $"sp_{seasonPack.UniqueId}_ep_{episode}";
        Version = seasonPack.Release ?? seasonPack.Filename;
        Completed = true;
        HearingImpaired = false;
        Corrected = false;
        DownloadUri = downloadUri;
        Language = language?.EnglishName ?? "Unknown";
        Discovered = seasonPack.Discovered;
        DownloadCount = seasonPack.DownloadCount;
        Source = seasonPack.Source.ToString();
        Qualities = VideoQualityFactory.ToDisplayNames(seasonPack.Qualities);
        HD = (seasonPack.Qualities & (VideoQuality.Q720P | VideoQuality.Q1080P | VideoQuality.Q2160P)) != VideoQuality.None;
        Release = seasonPack.Release;
    }

    /// <summary>
    /// Constructor for a specific catalog entry within a season pack.
    /// Each entry represents a single file in the ZIP (e.g., regular SRT vs dubtitle).
    /// </summary>
    public SubtitleDto(SeasonPackSubtitle seasonPack, SeasonPackEntry entry, string downloadUri, Culture.Model.Culture? language, int episode)
    {
        SubtitleId = $"sp_{seasonPack.UniqueId}_entry_{entry.UniqueId}";
        Version = entry.ReleaseGroup ?? seasonPack.Release ?? seasonPack.Filename;
        Completed = true;
        HearingImpaired = entry.FileName.Contains("dubtitle", StringComparison.OrdinalIgnoreCase);
        Corrected = false;
        DownloadUri = downloadUri;
        Language = language?.EnglishName ?? "Unknown";
        Discovered = seasonPack.Discovered;
        DownloadCount = seasonPack.DownloadCount;
        Source = seasonPack.Source.ToString();
        Qualities = VideoQualityFactory.ToDisplayNames(seasonPack.Qualities);
        HD = (seasonPack.Qualities & (VideoQuality.Q720P | VideoQuality.Q1080P | VideoQuality.Q2160P)) != VideoQuality.None;
        Release = seasonPack.Release;
    }

    /// <summary>
    /// Unique Id of the subtitle
    /// </summary>
    /// <example>1086727A-EB71-4B24-A209-7CF22374574D</example>
    [Required]
    public string SubtitleId { get; }

    /// <summary>
    /// Version of the subtitle
    /// </summary>
    /// <example>HDTV</example>
    [Required]
    public string Version { get; }

    [Required]
    public bool Completed { get; }

    [Required]
    public bool HearingImpaired { get; }

    [Required]
    public bool Corrected { get; }

    /// <summary>
    /// Whether this subtitle has HD quality (720P, 1080P, or 2160P).
    /// Derived from <see cref="Qualities"/>.
    /// </summary>
    [Required]
    public bool HD { get; }

    /// <summary>
    /// Url to download the subtitle
    /// </summary>
    /// <example>/download/1086727A-EB71-4B24-A209-7CF22374574D</example>
    [Required]
    public string DownloadUri { get; }

    /// <summary>
    /// Language of the subtitle (in English)
    /// </summary>
    /// <example>English</example>
    [Required]
    public string Language { get; }

    /// <summary>
    ///     When was the subtitle discovered in UTC
    /// </summary>
    /// <example>2022-04-02T05:16:45.4001274</example>
    [Required]
    public DateTime Discovered { get; }

    /// <summary>
    /// Number of times the subtitle was downloaded from the proxy
    /// </summary>
    /// <example>100</example>
    [Required]
    public long DownloadCount { get; }

    /// <summary>
    /// Source provider of the subtitle
    /// </summary>
    /// <example>Addic7ed</example>
    [Required]
    public string Source { get; }

    /// <summary>
    /// Available video qualities for this subtitle.
    /// Empty array when quality information is unavailable (e.g. legacy Addic7ed subtitles).
    /// </summary>
    /// <example>["720p","1080p"]</example>
    [Required]
    public string[] Qualities { get; }

    /// <summary>
    /// Full release name from the provider (e.g. filename without extension).
    /// Populated for SuperSubtitles; null for Addic7ed.
    /// </summary>
    /// <example>Show.S01E03.720p.BluRay.x264-GROUP</example>
    public string? Release { get; }
}
