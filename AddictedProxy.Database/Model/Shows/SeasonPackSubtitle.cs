using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AddictedProxy.Database.Model.Shared;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Model.Shows;

/// <summary>
/// Stores season pack subtitles from providers that support them (e.g. SuperSubtitles).
/// Season packs cover a whole season and are not linked to individual episodes.
/// </summary>
[Index(nameof(TvShowId), nameof(Season))]
[Index(nameof(Source), nameof(ExternalId), IsUnique = true)]
[Index(nameof(UniqueId), IsUnique = true)]
public class SeasonPackSubtitle : BaseEntity, IDiscoverableObject
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Unique identifier for external-facing APIs
    /// </summary>
    public Guid UniqueId { get; set; }

    /// <summary>
    /// Which show this season pack belongs to
    /// </summary>
    public long TvShowId { get; set; }

    [ForeignKey(nameof(TvShowId))]
    public virtual TvShow TvShow { get; set; } = null!;

    /// <summary>
    /// Season number from the provider
    /// </summary>
    public int Season { get; set; }

    /// <summary>
    /// Optional FK to the <see cref="Shows.Season"/> entity. Backfilled via one-time migration.
    /// </summary>
    public long? SeasonId { get; set; }

    [ForeignKey(nameof(SeasonId))]
    public virtual Season? SeasonEntity { get; set; }

    /// <summary>
    /// Number of times this season pack was downloaded
    /// </summary>
    public long DownloadCount { get; set; }

    /// <summary>
    /// Provider source (e.g. SuperSubtitles)
    /// </summary>
    public DataSource Source { get; set; }

    /// <summary>
    /// Provider-specific subtitle ID
    /// </summary>
    public long ExternalId { get; set; }

    /// <summary>
    /// Original filename from the provider
    /// </summary>
    public string Filename { get; set; } = null!;

    /// <summary>
    /// Language of the subtitle
    /// </summary>
    public string Language { get; set; } = null!;

    /// <summary>
    /// ISO language code (e.g. "en", "hu")
    /// </summary>
    [Column(TypeName = "VARCHAR")]
    [StringLength(7)]
    public string? LanguageIsoCode { get; set; }

    /// <summary>
    /// Release name from the provider
    /// </summary>
    public string? Release { get; set; }

    /// <summary>
    /// Optional first episode covered by this season pack.
    /// </summary>
    public int? RangeStart { get; set; }

    /// <summary>
    /// Optional last episode covered by this season pack.
    /// </summary>
    public int? RangeEnd { get; set; }

    /// <summary>
    /// Who uploaded this subtitle
    /// </summary>
    public string? Uploader { get; set; }

    /// <summary>
    /// When the subtitle was uploaded to the provider
    /// </summary>
    public DateTime? UploadedAt { get; set; }

    /// <summary>
    /// Bitmask of <see cref="VideoQuality"/> flags for this subtitle.
    /// </summary>
    public VideoQuality Qualities { get; set; } = VideoQuality.None;

    /// <summary>
    /// Serialized release group names from the provider
    /// </summary>
    public string? ReleaseGroups { get; set; }

    /// <summary>
    /// Path in S3/storage for cached subtitle file
    /// </summary>
    public string? StoragePath { get; set; }

    /// <summary>
    /// When the subtitle was cached to storage
    /// </summary>
    public DateTime? StoredAt { get; set; }

    /// <summary>
    /// Cataloged entries parsed from the ZIP archive
    /// </summary>
    public virtual ICollection<SeasonPackEntry> Entries { get; set; } = [];

    /// <inheritdoc />
    public DateTime Discovered { get; set; }
}
