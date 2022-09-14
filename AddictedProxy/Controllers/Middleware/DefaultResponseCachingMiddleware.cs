using Microsoft.Net.Http.Headers;

namespace AddictedProxy.Controllers.Middleware;

public class DefaultResponseCachingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        //Default cache response for 10 seconds
        context.Response.GetTypedHeaders().CacheControl =
            new CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(10)
            };
        context.Response.Headers[HeaderNames.Vary] =
            new[] { "Accept-Encoding" };

        //In case the request wasn't a success, don't cache the result
        context.Response.OnStarting(state =>
        {
            var httpContext = (HttpContext)state;
            if (httpContext.Response.StatusCode is not (>= 200 and <= 299) && httpContext.Response.GetTypedHeaders().CacheControl == null)
            {
                context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
                {
                    Public = false,
                    MaxAge = TimeSpan.Zero,
                    NoCache = true,
                    NoStore = true
                };
            }

            return Task.CompletedTask;
        }, context);

        await next(context);
    }
}