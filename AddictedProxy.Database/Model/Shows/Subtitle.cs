using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Model.Shows;

[Index(nameof(DownloadUri), IsUnique = true)]
[Index("EpisodeId", nameof(Language), nameof(Version), IsUnique = true)]
[Index(nameof(UniqueId), IsUnique = true)]
public class Subtitle : IDiscoverableObject
{
    [Key]
    public long Id { get; set; }

    public long EpisodeId { get; set; }

    [ForeignKey("EpisodeId")]
    public virtual Episode Episode { get; set; }

    public string Scene { get; set; }

    public int Version { get; set; }
    public bool Completed { get; set; }
    public bool HearingImpaired { get; set; }
    public bool Corrected { get; set; }
    public bool HD { get; set; }
    public Uri DownloadUri { get; set; }
    public string Language { get; set; }

    public string? StoragePath { get; set; }
    
    public DateTime? StoredAt { get; set; }

    public Guid UniqueId { get; set; } = Guid.NewGuid();

    /// <summary>
    ///     When was the subtitle discovered
    /// </summary>
    public DateTime Discovered { get; set; }
}