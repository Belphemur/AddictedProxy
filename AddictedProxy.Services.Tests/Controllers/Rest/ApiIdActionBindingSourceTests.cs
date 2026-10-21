using System.Reflection;
using AddictedProxy.Controllers.Rest;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace AddictedProxy.Services.Tests.Controllers.Rest;

[TestFixture]
public class ApiIdActionBindingSourceTests
{
    /// <summary>
    /// MediaController is not [ApiController], so route parameters only reach
    /// the base62 model binder (<c>ApiIdRouteModelBinderProvider</c> requires a
    /// Path binding source) when the action declares them [FromRoute]
    /// explicitly. A missing annotation silently degrades to the default Guid
    /// binder → 400 on every base62 id. Guard every action whose route
    /// template contains an apiId segment.
    /// </summary>
    [Test]
    public void MediaController_ActionsWithApiIdRoutes_BindGuidIdParametersFromRoute()
    {
        var offending = new List<string>();

        foreach (var method in typeof(MediaController).GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            var routeAttribute = method.GetCustomAttribute<RouteAttribute>();
            if (routeAttribute is null || !routeAttribute.Template.Contains(":apiId"))
            {
                continue;
            }

            foreach (var parameter in method.GetParameters())
            {
                if (parameter.ParameterType != typeof(Guid) && parameter.ParameterType != typeof(Guid?))
                {
                    continue;
                }

                if (parameter.GetCustomAttribute<FromRouteAttribute>() is null)
                {
                    offending.Add($"{method.Name}({parameter.Name}) on '{routeAttribute.Template}'");
                }
            }
        }

        offending.Should().BeEmpty("apiId-routed Guid parameters without [FromRoute] cannot reach the base62 model binder");
    }
}
