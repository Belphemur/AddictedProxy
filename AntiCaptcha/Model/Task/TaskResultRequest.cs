using System.Text.Json.Serialization;

namespace AntiCaptcha.Model.Task;

public record struct TaskResultRequest
{
    [JsonPropertyName("clientKey")] public string ClientKey { get; init; }
    [JsonPropertyName("taskId")] public long TaskId { get; init; }
};