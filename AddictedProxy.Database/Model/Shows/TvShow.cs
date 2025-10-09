#region

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AddictedProxy.Database.Model.Shared;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AddictedProxy.Database.Model.Shows;

[Index(nameof(ExternalId), IsUnique = true)]
[Index(nameof(UniqueId), IsUnique = true)]
[Index(nameof(TvdbId))]
public class TvShow : BaseEntity, IDiscoverableObject
{
    [Key]
    public long Id { get; set; }

    public Guid UniqueId { get; set; }

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
    /// Id in the Tmdb database for show/movie
    /// </summary>
    public int? TmdbId { get; set; }
    
    /// <summary>
    /// Id in the TvDb database for show
    /// </summary>
    public int? TvdbId { get; set; }
    
    /// <summary>
    /// Is the show completed. No more episode coming.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Is it a movie or a tv show
    /// </summary>
    public ShowType Type { get; set; } = ShowType.Show;

    /// <summary>
    /// Where was the show first discovered
    /// </summary>
    [DefaultValue(DataSource.Addic7ed)]
    public DataSource Source { get; set; } = DataSource.Addic7ed;
}