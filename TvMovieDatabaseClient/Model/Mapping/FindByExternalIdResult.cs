using System.Collections.Generic;
using System.Text.Json.Serialization;
using TvMovieDatabaseClient.Model.Movie.Search;
using TvMovieDatabaseClient.Model.Show.Search;

namespace TvMovieDatabaseClient.Model.Mapping;

/// <summary>
/// Response from TMDB's /find/{external_id} endpoint.
/// Used for resolving IMDB IDs to TMDB show/movie IDs.
/// </summary>
public class FindByExternalIdResult
{
    [JsonPropertyName("movie_results")]
    public List<MovieSearchResult> MovieResults { get; set; } = [];

    [JsonPropertyName("tv_results")]
    public List<ShowSearchResult> TvResults { get; set; } = [];
}
