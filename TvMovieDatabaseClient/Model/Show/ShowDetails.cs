using System.Collections.Generic;
using System.Text.Json.Serialization;
using TvMovieDatabaseClient.Model.Common;

namespace TvMovieDatabaseClient.Model.Show;

public class CreatedBy
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("credit_id")]
    public required string CreditId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("gender")]
    public int Gender { get; set; }

    [JsonPropertyName("profile_path")]
    public string? ProfilePath { get; set; }
}

public class Network
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("logo_path")]
    public string? LogoPath { get; set; }

    [JsonPropertyName("origin_country")]
    public required string OriginCountry { get; set; }
}

public class EpisodeData
{
    [JsonPropertyName("air_date")]
    public required string AirDate { get; set; }

    [JsonPropertyName("episode_number")]
    public int EpisodeNumber { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("overview")]
    public required string Overview { get; set; }

    [JsonPropertyName("production_code")]
    public string? ProductionCode { get; set; }

    [JsonPropertyName("runtime")]
    public int? Runtime { get; set; }

    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }

    [JsonPropertyName("show_id")]
    public int ShowId { get; set; }

    [JsonPropertyName("still_path")]
    public string? StillPath { get; set; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }
}

public class ShowDetails
{
    [JsonPropertyName("adult")]
    public bool Adult { get; set; }

    // TMDB returns null when the show has no backdrop; defaults to empty so
    // consumers passing it to non-nullable parameters keep working.
    [JsonPropertyName("backdrop_path")]
    public string BackdropPath { get; set; } = string.Empty;

    [JsonPropertyName("created_by")]
    public required List<CreatedBy> CreatedBy { get; set; }

    [JsonPropertyName("episode_run_time")]
    public required List<int> EpisodeRunTime { get; set; }

    [JsonPropertyName("first_air_date")]
    public required string FirstAirDate { get; set; }

    [JsonPropertyName("genres")]
    public required List<Genre> Genres { get; set; }

    [JsonPropertyName("homepage")]
    public string? Homepage { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("in_production")]
    public bool InProduction { get; set; }

    [JsonPropertyName("languages")]
    public required List<string> Languages { get; set; }

    [JsonPropertyName("last_air_date")]
    public required string LastAirDate { get; set; }

    [JsonPropertyName("last_episode_to_air")]
    public EpisodeData? LastEpisodeToAir { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("next_episode_to_air")]
    public EpisodeData? EpisodeData { get; set; }

    [JsonPropertyName("networks")]
    public required List<Network> Networks { get; set; }

    [JsonPropertyName("number_of_episodes")]
    public int? NumberOfEpisodes { get; set; }

    [JsonPropertyName("number_of_seasons")]
    public int? NumberOfSeasons { get; set; }

    [JsonPropertyName("origin_country")]
    public required List<string> OriginCountry { get; set; }

    [JsonPropertyName("original_language")]
    public required string OriginalLanguage { get; set; }

    [JsonPropertyName("original_name")]
    public required string OriginalName { get; set; }

    [JsonPropertyName("overview")]
    public required string Overview { get; set; }

    [JsonPropertyName("popularity")]
    public double Popularity { get; set; }

    // TMDB returns null when the show has no poster; defaults to empty so
    // consumers passing it to non-nullable parameters keep working.
    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; } = string.Empty;

    [JsonPropertyName("production_companies")]
    public required List<ProductionCompany> ProductionCompanies { get; set; }

    [JsonPropertyName("production_countries")]
    public required List<ProductionCountry> ProductionCountries { get; set; }

    [JsonPropertyName("seasons")]
    public required List<Season> Seasons { get; set; }

    [JsonPropertyName("spoken_languages")]
    public required List<SpokenLanguage> SpokenLanguages { get; set; }

    [JsonPropertyName("status")]
    public required string Status { get; set; }

    // TMDB returns null when the show has no tagline; defaults to empty so
    // consumers passing it to non-nullable parameters keep working.
    [JsonPropertyName("tagline")]
    public string Tagline { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }
}

public class Season
{
    [JsonPropertyName("air_date")]
    public required string AirDate { get; set; }

    [JsonPropertyName("episode_count")]
    public int EpisodeCount { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("overview")]
    public required string Overview { get; set; }

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }
}
