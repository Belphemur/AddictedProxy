#region

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

#endregion

namespace InversionOfControl.Service.Resilience;

/// <summary>
/// Shared HTTP resilience configuration used across all upstream HTTP clients.
/// </summary>
public static class ResilienceDefaults
{
    /// <summary>
    /// Configures the standard resilience handler with a circuit breaker that opens
    /// when the upstream returns server errors (5xx) or authentication/payment errors
    /// (401, 402, 403).
    /// </summary>
    public static IHttpClientBuilder AddSharedResilienceHandler(this IHttpClientBuilder builder, string pipelineName)
    {
        builder.AddResilienceHandler(pipelineName, pipelineBuilder =>
        {
            pipelineBuilder.AddRetry(new HttpRetryStrategyOptions
            {
                ShouldHandle = args => ValueTask.FromResult(
                    args.Outcome.Result is HttpResponseMessage response &&
                    ShouldTreatAsFailure(response.StatusCode)),
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                MaxDelay = TimeSpan.FromSeconds(30)
            });

            pipelineBuilder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                ShouldHandle = args => ValueTask.FromResult(
                    args.Outcome.Result is HttpResponseMessage response &&
                    ShouldTreatAsFailure(response.StatusCode)),
                SamplingDuration = TimeSpan.FromMinutes(1),
                FailureRatio = 0.5,
                MinimumThroughput = 10,
                BreakDuration = TimeSpan.FromMinutes(2)
            });

            pipelineBuilder.AddTimeout(new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(60)
            });
        });

        return builder;
    }

    private static bool ShouldTreatAsFailure(HttpStatusCode statusCode)
    {
        var status = (int)statusCode;
        return status is >= 500 and <= 599 or 401 or 402 or 403;
    }
}
