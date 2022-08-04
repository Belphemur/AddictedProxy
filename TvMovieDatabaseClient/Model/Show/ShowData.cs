// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);

using System.Collections.Generic;
using System.Text.Json.Serialization;

public record CreatedBy(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("credit_id")] string CreditId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("gender")] int Gender,
        [property: JsonPropertyName("profile_path")] string ProfilePath
    );

    public record Genre(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name
    );

    public record EpisodeData(
        [property: JsonPropertyName("air_date")] string AirDate,
        [property: JsonPropertyName("episode_number")] int EpisodeNumber,
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("overview")] string Overview,
        [property: JsonPropertyName("production_code")] string ProductionCode,
        [property: JsonPropertyName("runtime")] int Runtime,
        [property: JsonPropertyName("season_number")] int SeasonNumber,
        [property: JsonPropertyName("show_id")] int ShowId,
        [property: JsonPropertyName("still_path")] string StillPath,
        [property: JsonPropertyName("vote_average")] double VoteAverage,
        [property: JsonPropertyName("vote_count")] int VoteCount
    );

    public record Network(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("logo_path")] string LogoPath,
        [property: JsonPropertyName("origin_country")] string OriginCountry
    );

public record ProductionCompany(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("logo_path")] string LogoPath,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("origin_country")] string OriginCountry
    );

    public record ProductionCountry(
        [property: JsonPropertyName("iso_3166_1")] string Iso31661,
        [property: JsonPropertyName("name")] string Name
    );

    public record ShowData(
        [property: JsonPropertyName("adult")] bool Adult,
        [property: JsonPropertyName("backdrop_path")] string BackdropPath,
        [property: JsonPropertyName("created_by")] IReadOnlyList<CreatedBy> CreatedBy,
        [property: JsonPropertyName("episode_run_time")] IReadOnlyList<int> EpisodeRunTime,
        [property: JsonPropertyName("first_air_date")] string FirstAirDate,
        [property: JsonPropertyName("genres")] IReadOnlyList<Genre> Genres,
        [property: JsonPropertyName("homepage")] string Homepage,
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("in_production")] bool InProduction,
        [property: JsonPropertyName("languages")] IReadOnlyList<string> Languages,
        [property: JsonPropertyName("last_air_date")] string LastAirDate,
        [property: JsonPropertyName("last_episode_to_air")] EpisodeData EpisodeData,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("next_episode_to_air")] EpisodeData NextEpisodeToAir,
        [property: JsonPropertyName("networks")] IReadOnlyList<Network> Networks,
        [property: JsonPropertyName("number_of_episodes")] int NumberOfEpisodes,
        [property: JsonPropertyName("number_of_seasons")] int NumberOfSeasons,
        [property: JsonPropertyName("origin_country")] IReadOnlyList<string> OriginCountry,
        [property: JsonPropertyName("original_language")] string OriginalLanguage,
        [property: JsonPropertyName("original_name")] string OriginalName,
        [property: JsonPropertyName("overview")] string Overview,
        [property: JsonPropertyName("popularity")] double Popularity,
        [property: JsonPropertyName("poster_path")] string PosterPath,
        [property: JsonPropertyName("production_companies")] IReadOnlyList<ProductionCompany> ProductionCompanies,
        [property: JsonPropertyName("production_countries")] IReadOnlyList<ProductionCountry> ProductionCountries,
        [property: JsonPropertyName("seasons")] IReadOnlyList<Season> Seasons,
        [property: JsonPropertyName("spoken_languages")] IReadOnlyList<SpokenLanguage> SpokenLanguages,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("tagline")] string Tagline,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("vote_average")] double VoteAverage,
        [property: JsonPropertyName("vote_count")] int VoteCount
    );

    public record Season(
        [property: JsonPropertyName("air_date")] string AirDate,
        [property: JsonPropertyName("episode_count")] int EpisodeCount,
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("overview")] string Overview,
        [property: JsonPropertyName("poster_path")] string PosterPath,
        [property: JsonPropertyName("season_number")] int SeasonNumber
    );

    public record SpokenLanguage(
        [property: JsonPropertyName("english_name")] string EnglishName,
        [property: JsonPropertyName("iso_639_1")] string Iso6391,
        [property: JsonPropertyName("name")] string Name
    );

