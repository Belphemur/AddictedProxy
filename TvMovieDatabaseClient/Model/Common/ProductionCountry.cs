using System.Text.Json.Serialization;

namespace TvMovieDatabaseClient.Model.Common;

public class ProductionCountry
{
    [JsonPropertyName("iso_3166_1")]
    public required string Iso31661 { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
