using System.Text.Json.Serialization;

namespace TvMovieDatabaseClient.Model.Common;

public class Genre
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
