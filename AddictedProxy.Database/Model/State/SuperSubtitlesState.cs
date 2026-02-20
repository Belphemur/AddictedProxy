using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AddictedProxy.Database.Model.Shared;

namespace AddictedProxy.Database.Model.State;

/// <summary>
/// Stores the last known max subtitle ID from SuperSubtitles for incremental sync.
/// Only one row should exist in this table.
/// </summary>
[Table("SuperSubtitlesState")]
public class SuperSubtitlesState : BaseEntity
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// The highest subtitle ID seen from SuperSubtitles.
    /// Used as the cursor for <c>GetRecentSubtitles(since_id)</c>.
    /// </summary>
    public long MaxSubtitleId { get; set; }
}
