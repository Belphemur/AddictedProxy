using System.Text.Json.Serialization;

namespace AntiCaptcha.Model.Task;

public record struct SolutionResponse<T> where T : ISolutionData
{
    [JsonPropertyName("errorId")]
    public int ErrorId { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; }

    [JsonPropertyName("solution")]
    public T? Solution { get; init; }

    [JsonPropertyName("cost")]
    public string Cost { get; init; }

    [JsonPropertyName("ip")]
    public string Ip { get; init; }

    [JsonPropertyName("createTime")]
    public long CreateTime { get; init; }

    [JsonPropertyName("endTime")]
    public long EndTime { get; init; }

    [JsonPropertyName("solveCount")]
    public long SolveCount { get; init; }
}