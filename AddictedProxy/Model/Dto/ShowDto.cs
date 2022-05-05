using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Model.Dto;

/// <summary>
/// Represent the information relating to a show
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="NbSeasons"></param>
public record ShowDto(long Id, string Name, long NbSeasons)
{
    /// <summary>
    /// Unique ID of the show
    /// </summary>
    /// <example>1234</example>
    public long Id { get; init; } = Id;

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

    public ShowDto(TvShow show) : this(show.Id, show.Name, show.Seasons.Count)
    {
    }
}