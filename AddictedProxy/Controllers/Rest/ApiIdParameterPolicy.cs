using AddictedProxy.Model.Api;

#pragma warning disable CA1515 // Make types internal — registered by name (string) in RouteOptions.ConstraintMap.

namespace AddictedProxy.Controllers.Rest;

/// <summary>
/// Route parameter policy for external API identifiers, registered under the
/// name <c>apiId</c> in <see cref="Microsoft.AspNetCore.Routing.RouteOptions.ConstraintMap"/>
/// and used in templates as <c>{showId:apiId}</c>.
///
/// Dual role over the single <see cref="Base62Id"/> funnel:
/// <list type="bullet">
/// <item><see cref="IRouteConstraint.Match">Inbound validation</see>: a route
/// only matches when the segment decodes as a canonical base62 id — anything
/// else fails closed with the same UX as the previous <c>:guid</c>
/// constraint (404).</item>
/// <item><see cref="IOutboundParameterTransformer.TransformOutbound">Outbound
/// link generation</see>: <see cref="Guid"/> route values are encoded, so
/// <c>Url.RouteUrl</c> / sitemap builders emit base62 without per-call-site
/// changes; string values (already-encoded composite ids) pass through
/// unchanged.</item>
/// </list>
///
/// Model binding is decoupled: the <see cref="ApiIdRouteModelBinderProvider"/>
/// converts validated path values back to <see cref="Guid"/> for typed action
/// parameters, so controllers and services keep their <see cref="Guid"/>
/// signatures. Reference: <c>docs/2026-10-20-api-id-base62.md</c>.
/// </summary>
public class ApiIdParameterPolicy : IParameterPolicy, IRouteConstraint, IOutboundParameterTransformer
{
    /// <inheritdoc />
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (!values.TryGetValue(routeKey, out var value))
        {
            return false;
        }

        return routeDirection switch
        {
            // Link generation passes typed Guid values; transformers encode them afterwards.
            RouteDirection.UrlGeneration => value is Guid || (value is string s && Base62Id.TryDecode(s, out _)),
            // Incoming URLs are strings; only the canonical encoding matches.
            _ => value is string incoming && Base62Id.TryDecode(incoming, out _),
        };
    }

    /// <inheritdoc />
    public string? TransformOutbound(object? value)
    {
        return value switch
        {
            Guid guid => Base62Id.Encode(guid),
            string str => str,
            null => null,
            _ => value.ToString(),
        };
    }
}
