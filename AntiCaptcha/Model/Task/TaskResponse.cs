using System.Text.Json.Serialization;

namespace AntiCaptcha.Model.Task;

public record struct TaskResponse
{
    [JsonPropertyName("errorId")] public int ErrorId { get; init; }

    [JsonPropertyName("taskId")] public long TaskId { get; init; }
}