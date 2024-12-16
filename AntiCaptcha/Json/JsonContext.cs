using System.Text.Json.Serialization;
using AntiCaptcha.Model.Balance;
using AntiCaptcha.Model.Task;
using AntiCaptcha.Model.Task.Turnstile;

namespace AntiCaptcha.Json;

[JsonSerializable(typeof(SolutionResponse<TurnstileSolution>))]
[JsonSerializable(typeof(TaskRequest<TurnstileProxylessTask>))]
[JsonSerializable(typeof(TaskResponse))]
[JsonSerializable(typeof(TaskResultRequest))]
[JsonSerializable(typeof(BalanceResponse))]
[JsonSerializable(typeof(BalanceRequest))]
internal partial class JsonContext  : JsonSerializerContext
{
    
}