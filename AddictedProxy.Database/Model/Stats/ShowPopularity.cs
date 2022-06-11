using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AddictedProxy.Database.Model.Shows;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Model.Stats;

public class ShowPopularity
{
    public long TvShowId { get; set; }

    [ForeignKey("TvShowId")]
    public virtual TvShow TvShow { get; set; }
    public string Language { get; set; }
    
    public long RequestedCount { get; set; }
    
    public DateTime? LastRequestedDate { get; set; }
}