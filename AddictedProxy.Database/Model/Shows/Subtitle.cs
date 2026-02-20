#region

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AddictedProxy.Database.Model.Shared;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AddictedProxy.Database.Model.Shows;

[Index(nameof(DownloadUri), IsUnique = true)]
[Index(nameof(EpisodeId), nameof(Language), nameof(Version), IsUnique = false)]
[Index(nameof(UniqueId), IsUnique = true)]
[Index(nameof(Source), nameof(ExternalId), IsUnique = true)]
public class Subtitle : BaseEntity, IDiscoverableObject
{
    [Key]
    public long Id { get; set; }

    public long EpisodeId { get; set; }

    [ForeignKey(nameof(EpisodeId))]
    public virtual Episode Episode { get; set; }

    public string Scene { get; set; }

    public int Version { get; set; }
    public bool Completed { get; set; }
    public double CompletionPct { get; set; }
    public bool HearingImpaired { get; set; }
    public bool Corrected { get; set; }
    /// <summary>
    ///     Video qualities available for this subtitle.
    ///     Stored as a bitmask of <see cref="VideoQuality" /> flags.
    /// </summary>
    public VideoQuality Qualities { get; set; } = VideoQuality.None;

    /// <summary>
    ///     Full release name (e.g. filename minus extension) from the provider.
    ///     Populated for SuperSubtitles; null for Addic7ed.
    /// </summary>
    public string? Release { get; set; }

    /// <summary>
    ///     Legacy Addic7ed HD flag. Kept in the database until the
    ///     <c>BackportHdToQualitiesMigration</c> one-time job has run.
    /// </summary>
    [Obsolete("Use Qualities instead. Will be removed after BackportHdToQualitiesMigration completes.")]
    public bool HD { get; set; }

    public Uri DownloadUri { get; set; }
    public string Language { get; set; }

    [Column(TypeName = "VARCHAR")]
    [StringLength(7)]
    public string? LanguageIsoCode { get; set; }

    public string? StoragePath { get; set; }

    public DateTime? StoredAt { get; set; }

    public Guid UniqueId { get; set; }

    /// <summary>
    ///     When was the subtitle discovered
    /// </summary>
    public DateTime Discovered { get; set; }

    /// <summary>
    ///Number of time this sub was downloaded
    /// </summary>
    public long DownloadCount { get; set; }

    /// <summary>
    /// Source of the subtitle
    /// </summary>
    [DefaultValue(DataSource.Addic7ed)]
    public DataSource Source { get; set; } = DataSource.Addic7ed;

    /// <summary>
    /// Provider-specific external identifier for the subtitle.
    /// For Addic7ed, this is the download URI string.
    /// For SuperSubtitles, this is the upstream subtitle ID.
    /// </summary>
    public string? ExternalId { get; set; }
}