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
        HD = subtitle.HD;
        Corrected = subtitle.Completed;
        DownloadUri = downloadUri;
        Language = language?.EnglishName ?? "Unknown";
        Discovered = subtitle.Discovered;
        SubtitleId = subtitle.UniqueId.ToString();
        DownloadCount = subtitle.DownloadCount;
        Source = subtitle.Source.ToString();
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
}