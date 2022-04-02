using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Model.Shows;

[Index(nameof(ExternalId), IsUnique = true)]
public class TvShow : IDiscoverableObject
{
    [Key]
    public int Id { get; set; }

    public int ExternalId { get; set; }
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
}