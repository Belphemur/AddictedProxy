using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TvMovieDatabaseClient.Model.Show;

public class Genre
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class ShowGenre
{
    [JsonPropertyName("genres")]
    public List<Genre> Genres { get; set; }
}