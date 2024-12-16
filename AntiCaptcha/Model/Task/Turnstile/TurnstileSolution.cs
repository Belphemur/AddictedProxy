using System.Text.Json.Serialization;

namespace AntiCaptcha.Model.Task.Turnstile;

public record struct TurnstileSolution: ISolutionData
{
    [JsonPropertyName("token")] public string Token { get; init; }

    [JsonPropertyName("userAgent")] public string UserAgent { get; init; }
}