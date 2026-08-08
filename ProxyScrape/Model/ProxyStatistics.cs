using System.Text.Json.Serialization;

namespace ProxyScrape.Model;

public class ProxyStatistics
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("remaining_data")]
    public long RemainingData { get; init; }

    [JsonPropertyName("remaining_traffic")]
    public long RemainingTraffic { get; init; }

    [JsonPropertyName("used_data")]
    public long UsedData { get; init; }
}
