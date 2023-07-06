using System.Collections.Generic;
using System.Text.Json.Serialization;
using TvMovieDatabaseClient.Model.Common;

namespace TvMovieDatabaseClient.Model.Show;
public class ShowGenres
{
    [JsonPropertyName("genres")]
    public List<Genre> Genres { get; set; }
}