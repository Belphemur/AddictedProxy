using System.Collections.Generic;
using System.Text.Json.Serialization;
using TvMovieDatabaseClient.Model.Common;

namespace TvMovieDatabaseClient.Model.Movie;

public class BelongsToCollection
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath { get; set; }
}

public class MovieDetails
{
    [JsonPropertyName("adult")]
    public bool Adult { get; set; }

    // TMDB returns null when the movie has no backdrop; defaults to empty so
    // consumers passing it to non-nullable parameters keep working.
    [JsonPropertyName("backdrop_path")]
    public string BackdropPath { get; set; } = string.Empty;

    [JsonPropertyName("belongs_to_collection")]
    public BelongsToCollection? BelongsToCollection { get; set; }

    [JsonPropertyName("budget")]
    public int Budget { get; set; }

    [JsonPropertyName("genres")]
    public required List<Genre> Genres { get; set; }

    [JsonPropertyName("homepage")]
    public string? Homepage { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("imdb_id")]
    public string? ImdbId { get; set; }

    [JsonPropertyName("original_language")]
    public required string OriginalLanguage { get; set; }

    [JsonPropertyName("original_title")]
    public required string OriginalTitle { get; set; }

    [JsonPropertyName("overview")]
    public required string Overview { get; set; }

    [JsonPropertyName("popularity")]
    public double Popularity { get; set; }

    // TMDB returns null when the movie has no poster; defaults to empty so
    // consumers passing it to non-nullable parameters keep working.
    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; } = string.Empty;

    [JsonPropertyName("production_companies")]
    public required List<ProductionCompany> ProductionCompanies { get; set; }

    [JsonPropertyName("production_countries")]
    public required List<ProductionCountry> ProductionCountries { get; set; }

    [JsonPropertyName("release_date")]
    public required string ReleaseDate { get; set; }

    [JsonPropertyName("revenue")]
    public int Revenue { get; set; }

    [JsonPropertyName("runtime")]
    public int Runtime { get; set; }

    [JsonPropertyName("spoken_languages")]
    public List<SpokenLanguage>? SpokenLanguages { get; set; }

    [JsonPropertyName("status")]
    public required string Status { get; set; }

    // TMDB returns null when the movie has no tagline; defaults to empty so
    // consumers passing it to non-nullable parameters keep working.
    [JsonPropertyName("tagline")]
    public string Tagline { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("video")]
    public bool Video { get; set; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }
}
