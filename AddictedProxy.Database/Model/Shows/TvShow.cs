#region

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AddictedProxy.Database.Model.Shows;

[Index(nameof(ExternalId), IsUnique = true)]
[Index(nameof(UniqueId), IsUnique = true)]
[Index(nameof(TmdbId), IsUnique = true)]
public class TvShow : IDiscoverableObject
{
    [Key]
    public long Id { get; set; }
    
    public Guid UniqueId { get; set; } = Guid.NewGuid();

    public long ExternalId { get; set; }
    public string Name { get; set; }

    public virtual IList<Episode> Episodes { get; set; }
    public virtual IList<Season> Seasons { get; set; }

    public DateTime LastUpdated { get; set; }

    /// <summary>
    ///     When was the last time we refreshed the seasons of the show
    /// </summary>
    public DateTime? LastSeasonRefreshed { get; set; }

    /// <summary>
    ///     When was the show discovered
    /// </summary>
    public DateTime Discovered { get; set; }

    /// <summary>
    /// Priority of the show when searching
    /// </summary>
    public int Priority { get; set; } = 0;
    
    /// <summary>
    /// Id in the Tmdb database for shows
    /// </summary>
    public int? TmdbId { get; set; }
    
    /// <summary>
    /// Is the show completed. No more episode coming.
    /// </summary>
    public bool IsCompleted { get; set; }
}