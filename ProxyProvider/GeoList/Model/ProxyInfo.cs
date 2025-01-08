using System.Text.Json.Serialization;

namespace ProxyProvider.GeoList.Model;

public class ProxyInfo
{
    [JsonPropertyName("_id")]
    public required string Id { get; init; }

    [JsonPropertyName("ip")]
    public required string Ip { get; init; }

    [JsonPropertyName("anonymityLevel")]
    public required string AnonymityLevel { get; init; }

    [JsonPropertyName("asn")]
    public required string Asn { get; init; }

    [JsonPropertyName("city")]
    public required string City { get; init; }

    [JsonPropertyName("country")]
    public required string Country { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("google")]
    public bool Google { get; init; }

    [JsonPropertyName("isp")]
    public required string Isp { get; init; }

    [JsonPropertyName("lastChecked")]
    public int LastChecked { get; init; }

    [JsonPropertyName("latency")]
    public double Latency { get; init; }

    [JsonPropertyName("org")]
    public required string Org { get; init; }

    [JsonPropertyName("port")]
    public required string Port { get; init; }

    [JsonPropertyName("protocols")]
    public required List<string> Protocols { get; init; }

    [JsonPropertyName("speed")]
    public int Speed { get; init; }

    [JsonPropertyName("upTime")]
    public double UpTime { get; init; }

    [JsonPropertyName("upTimeSuccessCount")]
    public int UpTimeSuccessCount { get; init; }

    [JsonPropertyName("upTimeTryCount")]
    public int UpTimeTryCount { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; init; }

    [JsonPropertyName("responinitime")]
    public int Responinitime { get; init; }
}

public class Response
{
    [JsonPropertyName("data")]
    public required ProxyInfo[] Data { get; init; }

    [JsonPropertyName("total")]
    public int Total { get; init; }

    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("limit")]
    public int Limit { get; init; }
}