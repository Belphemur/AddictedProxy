using System.ComponentModel.DataAnnotations.Schema;

namespace AddictedProxy.Model.Shows;

public class Season
{
    public int Id { get; set; }
    public int TvShowId { get; set; }

    [ForeignKey(nameof(TvShowId))]
    public virtual TvShow TvShow { get; set; }
    /// <summary>
    /// Number associated with the season
    /// </summary>
    public int Number { get; set; }
}