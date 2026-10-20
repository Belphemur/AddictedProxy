using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TvMovieDatabaseClient.Model.Movie.Search;

// This model is constructed with object initializers outside this project
// (AddictedProxy.Services.Tests), so only the identity fields can be
// `required`; the remaining always-returned fields get honest defaults.
public class MovieSearchResult
{
    [JsonPropertyName("adult")]
    public bool Adult { get; set; }

    // TMDB returns null when the movie has no backdrop; defaults to empty so
    // consumers passing it to non-nullable parameters keep working.
    [JsonPropertyName("backdrop_path")]
    public string BackdropPath { get; set; } = string.Empty;

    [JsonPropertyName("genre_ids")]
    public List<int> GenreIds { get; set; } = [];

    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("original_language")]
    public string OriginalLanguage { get; set; } = string.Empty;

    [JsonPropertyName("original_title")]
    public string OriginalTitle { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("popularity")]
    public double Popularity { get; set; }

    // TMDB returns null when the movie has no poster; defaults to empty so
    // consumers passing it to non-nullable parameters keep working.
    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; } = string.Empty;

    [JsonPropertyName("release_date")]
    public string ReleaseDate { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("video")]
    public bool Video { get; set; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }
}
