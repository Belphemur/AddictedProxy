using System.Text.Json.Serialization;

namespace ProxyScrape.Model;

public class ProxyStatistics
{
    [JsonPropertyName("remaining_data")]
    public long RemainingData { get; init; }

    [JsonPropertyName("used_data")]
    public long UsedData { get; init; }
}