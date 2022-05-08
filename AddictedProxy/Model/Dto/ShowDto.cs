using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Model.Dto;

/// <summary>
/// Represent the information relating to a show
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="NbSeasons"></param>
public record ShowDto(Guid Id, string Name, long NbSeasons)
{
    /// <summary>
    /// Unique ID of the show
    /// </summary>
    /// <example>E9C1FA23-55AF-4711-8E34-3B31E2A75533</example>
    public Guid Id { get; init; } = Id;

    /// <summary>
    /// Name of the show
    /// </summary>
    /// <example>Wellington Paranormal</example>
    public string Name { get; init; } = Name;

    /// <summary>
    /// How many season the show has
    /// </summary>
    /// <example>5</example>
    public long NbSeasons { get; init; } = NbSeasons;

    public ShowDto(TvShow show) : this(show.UniqueId, show.Name, show.Seasons.Count)
    {
    }
}