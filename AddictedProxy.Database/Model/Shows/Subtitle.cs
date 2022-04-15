using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Model.Shows;

[Index(nameof(DownloadUri), IsUnique = true)]
[Index("EpisodeId", nameof(Language), nameof(Version), IsUnique = true)]
public class Subtitle : IDiscoverableObject
{
    [Key]
    public int Id { get; set; }

    public int EpisodeId { get; set; }

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

    /// <summary>
    ///     When was the subtitle discovered
    /// </summary>
    public DateTime Discovered { get; set; }
}