using System.Text.Json.Serialization;
using TvMovieDatabaseClient.Model.Search;
using TvMovieDatabaseClient.Model.Show;

namespace TvMovieDatabaseClient.Model;

[JsonSerializable(typeof(ShowData))]
[JsonSerializable(typeof(Pagination<ShowSearchResult>))]
internal partial class JsonContext : JsonSerializerContext
{
}