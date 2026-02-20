using System.Text.Json.Serialization;
using TvMovieDatabaseClient.Model.Mapping;
using TvMovieDatabaseClient.Model.Movie;
using TvMovieDatabaseClient.Model.Movie.Search;
using TvMovieDatabaseClient.Model.Show;
using TvMovieDatabaseClient.Model.Show.Search;

namespace TvMovieDatabaseClient.Model;

[JsonSerializable(typeof(ShowDetails))]
[JsonSerializable(typeof(Pagination<ShowSearchResult>))]
[JsonSerializable(typeof(Pagination<MovieSearchResult>))]
[JsonSerializable(typeof(MovieDetails))]
[JsonSerializable(typeof(ExternalIds))]
[JsonSerializable(typeof(FindByExternalIdResult))]
[JsonSerializable(typeof(ShowGenres))]
internal partial class JsonContext : JsonSerializerContext
{
}