using System.Text.Json.Serialization;
using TvMovieDatabaseClient.Model.Search;

namespace TvMovieDatabaseClient.Model;

[JsonSerializable(typeof(ShowData))]
[JsonSerializable(typeof(Pagination<SearchResult>))]
internal partial class JsonContext : JsonSerializerContext
{
}