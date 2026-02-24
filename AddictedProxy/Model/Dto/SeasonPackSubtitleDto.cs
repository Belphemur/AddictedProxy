using System.ComponentModel.DataAnnotations;
using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Model.Dto;

/// <summary>
/// Season pack subtitle information
/// </summary>
public class SeasonPackSubtitleDto
{
    public SeasonPackSubtitleDto(SeasonPackSubtitle seasonPack, string downloadUri, Culture.Model.Culture? language)
    {
        SubtitleId = $"sp_{seasonPack.UniqueId}";
        Language = language?.EnglishName ?? "Unknown";
        Version = seasonPack.Release ?? seasonPack.Filename;
        Uploader = seasonPack.Uploader;
        UploadedAt = seasonPack.UploadedAt;
        Qualities = VideoQualityFactory.ToDisplayNames(seasonPack.Qualities);
        Source = seasonPack.Source.ToString();
        DownloadUri = downloadUri;
        DownloadCount = seasonPack.DownloadCount;
    }

    /// <summary>
    /// Unique Id of the season pack subtitle (prefixed with sp_)
    /// </summary>
    /// <example>sp_1086727A-EB71-4B24-A209-7CF22374574D</example>
    [Required]
    public string SubtitleId { get; }

    /// <summary>
    /// Language of the subtitle (in English)
    /// </summary>
    /// <example>English</example>
    [Required]
    public string Language { get; }

    /// <summary>
    /// Version/release name of the season pack
    /// </summary>
    /// <example>WEBRip.NTb</example>
    [Required]
    public string Version { get; }

    /// <summary>
    /// Who uploaded this subtitle
    /// </summary>
    /// <example>NTb</example>
    public string? Uploader { get; }

    /// <summary>
    /// When the subtitle was uploaded to the provider
    /// </summary>
    public DateTime? UploadedAt { get; }

    /// <summary>
    /// Available video qualities for this season pack
    /// </summary>
    /// <example>["720p","1080p"]</example>
    [Required]
    public string[] Qualities { get; }

    /// <summary>
    /// Source provider of the subtitle
    /// </summary>
    /// <example>SuperSubtitles</example>
    [Required]
    public string Source { get; }

    /// <summary>
    /// Url to download the season pack (ZIP archive)
    /// </summary>
    /// <example>/subtitles/download/sp_1086727A-EB71-4B24-A209-7CF22374574D</example>
    [Required]
    public string DownloadUri { get; }

    /// <summary>
    /// Number of times the season pack was downloaded from the proxy
    /// </summary>
    /// <example>42</example>
    [Required]
    public long DownloadCount { get; }
}
