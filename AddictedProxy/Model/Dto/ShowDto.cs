using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AddictedProxy.Culture.Extensions;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Api;

namespace AddictedProxy.Model.Dto;

/// <summary>
/// Represent the information relating to a show
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="NbSeasons"></param>
/// <param name="Seasons"></param>
/// <param name="TvDbId"></param>
/// <param name="TmdbId"></param>
public record ShowDto(Guid Id, string Name, int NbSeasons, int[] Seasons, int? TvDbId, int? TmdbId)
{
    /// <summary>
    /// Unique ID of the show
    /// </summary>
    /// <example>775rJjCDufZ0nuSVDeriR1</example>
    [Required]
    [ApiId]
    [JsonConverter(typeof(GuidBase62JsonConverter))]
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
    /// <summary>
    /// ID of the show in TMDB if available
    /// </summary>
    /// <example>80475</example>
    public int? TmdbId { get; init; } = TmdbId;
    
    /// <summary>
    /// Slug to be used in URL for the show
    /// <example>wellington-paranormal</example>
    /// </summary>
    [Required]
    public string Slug => Name.ToSlug();

    /// <summary>
    /// Factory from an entity. Kept off the constructors so the record has
    /// exactly one (the JSON binding constructor) — see
    /// docs/2026-10-20-api-id-base62.md.
    /// </summary>
    public static ShowDto From(TvShow show) =>
        new(show.UniqueId, show.Name, show.Seasons.Count, show.Seasons.Select(season => season.Number).ToArray(), show.TvdbId, show.TmdbId);
}