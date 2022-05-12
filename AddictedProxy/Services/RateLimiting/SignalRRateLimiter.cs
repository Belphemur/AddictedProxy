using AspNetCoreRateLimit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AddictedProxy.Services.RateLimiting;

public class SignalRRateLimiter : IHubFilter
{
    private readonly ILogger<SignalRRateLimiter> _logger;
    private readonly IRateLimitProcessor _processor;

    public SignalRRateLimiter(
        IProcessingStrategy processingStrategy,
        IOptions<IpRateLimitOptions> options,
        IIpPolicyStore policyStore,
        ILogger<SignalRRateLimiter> logger)
    {
        _logger = logger;
        _processor = new IpRateLimitProcessor(options.Value, policyStore, processingStrategy);
    }

    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var httpContext = invocationContext.Context.GetHttpContext();
        var ip = httpContext!.Connection.RemoteIpAddress!.ToString();
        var client = new ClientRequestIdentity
        {
            ClientIp = ip,
            Path = invocationContext.HubMethodName,
            HttpVerb = "ws",
            ClientId = invocationContext.Context.UserIdentifier
        };
        foreach (var rule in await _processor.GetMatchingRulesAsync(client))
        {
            var counter = await _processor.ProcessRequestAsync(client, rule);
            _logger.LogInformation("time: {0}, count: {1}", counter.Timestamp, counter.Count);

            if (!(counter.Count > rule.Limit))
            {
                continue;
            }

            var retry = counter.Timestamp.RetryAfterFrom(rule);
            throw new HubException($"call limit {retry}");
        }

        return await next(invocationContext);
    }
}