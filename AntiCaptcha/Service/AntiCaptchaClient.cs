using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AntiCaptcha.Json;
using AntiCaptcha.Model.Config;
using AntiCaptcha.Model.Error;
using AntiCaptcha.Model.Task;
using AntiCaptcha.Model.Task.Turnstile;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AntiCaptcha.Service;

public class AntiCaptchaClient : IAntiCaptchaClient
{
    private readonly IOptions<AntiCaptchaConfig> _config;
    private readonly ILogger<AntiCaptchaClient> _logger;
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        TypeInfoResolver = JsonTypeInfoResolver.Combine(JsonContext.Default, new DefaultJsonTypeInfoResolver())
    };

    public AntiCaptchaClient(IOptions<AntiCaptchaConfig> config, ILogger<AntiCaptchaClient> logger, HttpClient client)
    {
        _config = config;
        _logger = logger;
        _client = client;
    }

    /// <summary>
    /// Solve a Turnstile task using AntiCaptcha Proxyless API
    /// </summary>
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="SolvingException">If couldn't solve the captcha</exception>
    public async Task<SolutionResponse<TurnstileSolution>?> SolveTurnstileProxylessAsync(TurnstileProxylessTask task, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new TaskRequest<TurnstileProxylessTask>
            {
                ClientKey = _config.Value.ClientKey,
                Task = task
            };

            var response = await _client.PostAsJsonAsync("createTask", request, JsonSerializerOptions, cancellationToken);
            var taskResponse = await response
                .EnsureSuccessStatusCode()
                .Content.ReadFromJsonAsync<TaskResponse>(JsonSerializerOptions, cancellationToken);

            if (taskResponse.ErrorId > 0)
            {
                _logger.LogWarning("Failed to create a task: {ErrorId}", taskResponse.ErrorId);
                throw new SolvingException("Can't create a task");
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromMinutes(1));

            while (!cts.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(750), cts.Token);
                var solutionResponse = await _client.PostAsJsonAsync("getTaskResult", new TaskResultRequest { ClientKey = _config.Value.ClientKey, TaskId = taskResponse.TaskId }, JsonSerializerOptions, cts.Token);
                var solution = await solutionResponse
                    .EnsureSuccessStatusCode()
                    .Content.ReadFromJsonAsync<SolutionResponse<TurnstileSolution>>(JsonSerializerOptions, cts.Token);
                if (solution.Status == "ready" || solution.Solution != default)
                {
                    return solution;
                }
            }

            return null;
        }
        catch (OperationCanceledException e)
        {
            _logger.LogError(e, "Took too long to solve Turnstile task");
            throw new SolvingException("Took too long to solve Turnstile task", e);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to solve Turnstile task");
            throw new SolvingException("Couldn't solve the turnstile captcha", e);
        }
    }
}