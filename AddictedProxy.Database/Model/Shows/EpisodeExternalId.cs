using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AddictedProxy.Database.Model.Shared;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Model.Shows;

/// <summary>
/// Maps a provider-specific external ID to an <see cref="Episode"/>.
/// One external ID per provider per episode.
/// </summary>
[Index(nameof(EpisodeId), nameof(Source), IsUnique = true)]
[Index(nameof(Source), nameof(ExternalId), IsUnique = true)]
public class EpisodeExternalId : BaseEntity
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// The episode this external ID belongs to
    /// </summary>
    public long EpisodeId { get; set; }

    [ForeignKey(nameof(EpisodeId))]
    public virtual Episode Episode { get; set; } = null!;

    /// <summary>
    /// The provider that owns this external ID
    /// </summary>
    public DataSource Source { get; set; }

    /// <summary>
    /// The provider-specific ID (e.g. Addic7ed episode ID, SuperSubtitles episode ID)
    /// </summary>
    public string ExternalId { get; set; } = null!;
}
