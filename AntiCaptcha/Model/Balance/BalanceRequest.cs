using System.Text.Json.Serialization;

namespace AntiCaptcha.Model.Balance;

public record struct BalanceRequest
{
    [JsonPropertyName("clientKey")] public string ClientKey { get; init; }
}