using System.ComponentModel.DataAnnotations;
using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Model.Dto;

/// <summary>
/// Represent the information relating to a show
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="NbSeasons"></param>
public record ShowDto(Guid Id, string Name, int NbSeasons, int[] Seasons, int? TvDbId)
{
    /// <summary>
    /// Unique ID of the show
    /// </summary>
    /// <example>E9C1FA23-55AF-4711-8E34-3B31E2A75533</example>
    [Required]
    public Guid Id { get; init; } = Id;

    /// <summary>
    /// Name of the show
    /// </summary>
    /// <example>Wellington Paranormal</example>
    [Required]
    public string Name { get; init; } = Name;

    /// <summary>
    /// How many season the show has
    /// </summary>
    /// <example>5</example>
    [Required]
    public int NbSeasons { get; init; } = NbSeasons;

    /// <summary>
    /// Seasons available in ascending order
    /// </summary>
    /// <example>[2,3,4,5,6]</example>
    [Required]
    public int[] Seasons { get; init; } = Seasons.OrderBy(season => season).ToArray();

    /// <summary>
    /// Id of the show on the TvDB
    /// </summary>
    /// <example>344280</example>
    public int? TvDbId { get; init; } = TvDbId;

    public ShowDto(TvShow show) : this(show.UniqueId, show.Name, show.Seasons.Count, show.Seasons.Select(season => season.Number).ToArray(), show.TvdbId)
    {
    }
}