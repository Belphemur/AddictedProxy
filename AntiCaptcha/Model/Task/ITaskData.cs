using System.Text.Json.Serialization;

namespace AntiCaptcha.Model.Task;

public interface ITaskData
{
    [JsonPropertyName("type")] 
    public string Type { get; }
}