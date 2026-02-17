using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AddictedProxy.Database.Model.Shared;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Model.Shows;

/// <summary>
/// Maps a provider-specific external ID to a <see cref="TvShow"/>.
/// One external ID per provider per show.
/// </summary>
[Index(nameof(TvShowId), nameof(Source), IsUnique = true)]
[Index(nameof(Source), nameof(ExternalId), IsUnique = true)]
public class ShowExternalId : BaseEntity
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// The show this external ID belongs to
    /// </summary>
    public long TvShowId { get; set; }

    [ForeignKey(nameof(TvShowId))]
    public virtual TvShow TvShow { get; set; } = null!;

    /// <summary>
    /// The provider that owns this external ID
    /// </summary>
    public DataSource Source { get; set; }

    /// <summary>
    /// The provider-specific ID (e.g. Addic7ed show ID, SuperSubtitles show ID)
    /// </summary>
    public string ExternalId { get; set; } = null!;
}
