using System.Text.Json.Serialization;

namespace AntiCaptcha.Model.Task;

public record struct TaskRequest<T> where T : ITaskData
{
    [JsonPropertyName("clientKey")] public string ClientKey { get; init; }
    [JsonPropertyName("task")] public T Task { get; init; }
}