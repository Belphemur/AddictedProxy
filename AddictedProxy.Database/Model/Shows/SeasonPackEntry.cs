using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AddictedProxy.Database.Model.Shared;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Model.Shows;

/// <summary>
/// Represents a single file entry inside a season pack ZIP archive.
/// Each entry maps to one episode SRT extracted from the ZIP.
/// </summary>
[Index(nameof(SeasonPackSubtitleId), nameof(FileName), IsUnique = true)]
[Index(nameof(SeasonPackSubtitleId), nameof(EpisodeNumber))]
public class SeasonPackEntry : BaseEntity
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// FK to the parent season pack
    /// </summary>
    public long SeasonPackSubtitleId { get; set; }

    [ForeignKey(nameof(SeasonPackSubtitleId))]
    public virtual SeasonPackSubtitle SeasonPackSubtitle { get; set; } = null!;

    /// <summary>
    /// Episode number parsed from the filename via S\d{2}E(\d{2,3})
    /// </summary>
    public int EpisodeNumber { get; set; }

    /// <summary>
    /// Original filename of the entry inside the ZIP
    /// </summary>
    public string FileName { get; set; } = null!;

    /// <summary>
    /// Episode title parsed from the filename (text between SxxExx and first release marker), if available
    /// </summary>
    public string? EpisodeTitle { get; set; }

    /// <summary>
    /// Comma-separated release group names parsed from the filename
    /// </summary>
    public string? ReleaseGroup { get; set; }
}
