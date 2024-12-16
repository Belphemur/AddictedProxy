using System.Text.Json.Serialization;
using AntiCaptcha.Model.Task;
using AntiCaptcha.Model.Task.Turnstile;

namespace AntiCaptcha.Json;

[JsonSerializable(typeof(SolutionResponse<TurnstileSolution>))]
[JsonSerializable(typeof(TaskRequest<TurnstileProxylessTask>))]
[JsonSerializable(typeof(TaskResponse))]
[JsonSerializable(typeof(TaskResultRequest))]
internal partial class JsonContext  : JsonSerializerContext
{
    
}