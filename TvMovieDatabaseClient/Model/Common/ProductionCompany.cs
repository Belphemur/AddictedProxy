using System.Text.Json.Serialization;

namespace TvMovieDatabaseClient.Model.Common;

public class ProductionCompany
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("logo_path")]
    public string? LogoPath { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("origin_country")]
    public required string OriginCountry { get; set; }
}
