#region

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AddictedProxy.Database.Model.Shared;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AddictedProxy.Database.Model.Shows;

[Index(nameof(TvShowId), nameof(Season), nameof(Number), IsUnique = true)]
public class Episode : BaseEntity, IDiscoverableObject
{
    [Key]
    public long Id { get; set; }

    public long TvShowId { get; set; }

    [ForeignKey(nameof(TvShowId))]
    public virtual TvShow TvShow { get; set; }

    public int Season { get; set; }
    public int Number { get; set; }
    public string Title { get; set; }
    public IList<Subtitle> Subtitles { get; set; }
    public virtual IList<EpisodeExternalId> ExternalIds { get; set; } = [];

    /// <summary>
    ///     When was the episode discovered
    /// </summary>
    public DateTime Discovered { get; set; }
}