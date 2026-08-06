#region

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

#endregion

namespace InversionOfControl.Service.Resilience;

/// <summary>
/// Shared HTTP resilience configuration used across all upstream HTTP clients.
/// </summary>
public static class ResilienceDefaults
{
    private const string RequestMessagePropertyKey = "Resilience.Http.RequestMessage";

    /// <summary>
    /// Configures the standard resilience handler with retry, circuit breaker, and timeout.
    /// Retries only safe HTTP methods (GET, HEAD, OPTIONS, TRACE) unless the caller has
    /// explicitly marked the request as idempotent; treats 5xx and auth/payment errors
    /// (401, 402, 403) as failures, and also retries transient transport/timeout failures.
    /// </summary>
    public static IHttpClientBuilder AddSharedResilienceHandler(this IHttpClientBuilder builder, string pipelineName, TimeSpan? requestTimeout = null)
    {
        builder.AddResilienceHandler(pipelineName, pipelineBuilder =>
        {
            var retryPredicate = BuildFailurePredicate();

            pipelineBuilder.AddRetry(new HttpRetryStrategyOptions
            {
                ShouldHandle = args => ValueTask.FromResult(IsRetryable(args, retryPredicate)),
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                MaxDelay = TimeSpan.FromSeconds(30)
            });

            var circuitBreakerPredicate = BuildFailurePredicate();

            pipelineBuilder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                ShouldHandle = args => ValueTask.FromResult(IsFailure(args, circuitBreakerPredicate)),
                SamplingDuration = TimeSpan.FromMinutes(1),
                FailureRatio = 0.5,
                MinimumThroughput = 10,
                BreakDuration = TimeSpan.FromMinutes(2)
            });

            pipelineBuilder.AddTimeout(new HttpTimeoutStrategyOptions
            {
                Timeout = requestTimeout ?? TimeSpan.FromSeconds(60)
            });
        });

        return builder;
    }

    private static Predicate<Outcome<HttpResponseMessage>> BuildFailurePredicate()
    {
        return new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>()
            .Handle<TimeoutRejectedException>()
            .HandleResult(response => ShouldTreatAsFailure(response.StatusCode))
            .Build();
    }

    private static bool IsRetryable(RetryPredicateArguments<HttpResponseMessage> args, Predicate<Outcome<HttpResponseMessage>> predicate)
    {
        if (!predicate(args.Outcome))
        {
            return false;
        }

        if (!args.Context.Properties.TryGetValue(new ResiliencePropertyKey<HttpRequestMessage>(RequestMessagePropertyKey), out var request))
        {
            return false;
        }

        return IsSafeRequest(request) || HasIdempotencyKey(request);
    }

    private static bool IsFailure(CircuitBreakerPredicateArguments<HttpResponseMessage> args, Predicate<Outcome<HttpResponseMessage>> predicate)
    {
        if (!predicate(args.Outcome))
        {
            return false;
        }

        if (!args.Context.Properties.TryGetValue(new ResiliencePropertyKey<HttpRequestMessage>(RequestMessagePropertyKey), out var request))
        {
            return true;
        }

        return IsSafeRequest(request) || HasIdempotencyKey(request);
    }

    private static bool IsSafeRequest(HttpRequestMessage request)
    {
        return request.Method == HttpMethod.Get
               || request.Method == HttpMethod.Head
               || request.Method == HttpMethod.Options
               || request.Method == HttpMethod.Trace;
    }

    private static bool HasIdempotencyKey(HttpRequestMessage request)
    {
        return request.Headers.Contains("Idempotency-Key");
    }

    private static bool ShouldTreatAsFailure(HttpStatusCode statusCode)
    {
        var status = (int)statusCode;
        return status is >= 500 and <= 599 or 401 or 402 or 403;
    }
}
