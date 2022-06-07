using AddictedProxy.Controllers.Rest;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Provider.Subtitle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace AddictedProxy.Controllers.Middleware;

public class SubtitleIncrementDownloadMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        const string subtitleDownloadRoute = "/subtitles/download/";
        if (!context.Request.Path.Value?.StartsWith(subtitleDownloadRoute) ?? false)
        {
            await next(context);
            return;
        }

        await next(context);

        if (context.Response.StatusCode is not (>= 200 and <= 299))
        {
            return;
        }

        var subtitleRouteParam = context.Request.Path.Value?[subtitleDownloadRoute.Length..];
        if (subtitleRouteParam == null)
        {
            return;
        }

        if (!Guid.TryParse(subtitleRouteParam, out var subtitleId))
        {
            return;
        }
        var subtitleRepository = context.RequestServices.GetRequiredService<ISubtitleRepository>();

        var subtitle = await subtitleRepository.GetSubtitleByGuidAsync(subtitleId);
        if (subtitle == null)
        {
            return;
        }

        var subtitleCounterUpdater = context.RequestServices.GetRequiredService<SubtitleCounterUpdater>();

        await subtitleCounterUpdater.IncrementSubtitleCountAsync(subtitle, default);
    }
}