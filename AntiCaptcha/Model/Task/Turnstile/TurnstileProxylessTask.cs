using System.Text.Json.Serialization;

namespace AntiCaptcha.Model.Task.Turnstile;

public record struct TurnstileProxylessTask : ITaskData
{
    public string Type => "TurnstileTaskProxyless";
    [JsonPropertyName("websiteURL")] public string WebsiteUrl { get; init; }
    [JsonPropertyName("websiteKey")] public string WebsiteKey { get; init; }
    [JsonPropertyName("action")] public string? Action { get; init; }
    [JsonPropertyName("turnstileCData")] public string? TurnstileCData { get; init; }
};