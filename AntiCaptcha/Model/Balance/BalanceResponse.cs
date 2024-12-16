
using System.Text.Json.Serialization;

namespace AntiCaptcha.Model.Balance;

public record struct BalanceResponse
{
    [JsonPropertyName("errorId")]
    public int ErrorId { get; init; }

    [JsonPropertyName("balance")]
    public decimal Balance { get; init; }

    [JsonPropertyName("captchaCredits")]
    public long CaptchaCredits { get; init; }
}