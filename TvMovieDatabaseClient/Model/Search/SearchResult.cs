using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TvMovieDatabaseClient.Model.Search;

public record SearchResult(
    [property: JsonPropertyName("backdrop_path")]
    string BackdropPath,
    [property: JsonPropertyName("first_air_date")]
    string FirstAirDate,
    [property: JsonPropertyName("genre_ids")]
    IReadOnlyList<int> GenreIds,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("origin_country")]
    IReadOnlyList<string> OriginCountry,
    [property: JsonPropertyName("original_language")]
    string OriginalLanguage,
    [property: JsonPropertyName("original_name")]
    string OriginalName,
    [property: JsonPropertyName("overview")]
    string Overview,
    [property: JsonPropertyName("popularity")]
    double Popularity,
    [property: JsonPropertyName("poster_path")]
    string PosterPath,
    [property: JsonPropertyName("vote_average")]
    double VoteAverage,
    [property: JsonPropertyName("vote_count")]
    int VoteCount
);