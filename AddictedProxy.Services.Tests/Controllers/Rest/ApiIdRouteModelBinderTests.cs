using AddictedProxy.Controllers.Rest;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Routing;

namespace AddictedProxy.Services.Tests.Controllers.Rest;

[TestFixture]
public class ApiIdRouteModelBinderTests
{
    private static readonly Guid Sample = new("E9C1FA23-55AF-4711-8E34-3B31E2A75533");
    private const string SampleBase62 = "775rJjCDufZ0nuSVDeriR1";

    private static ModelBindingContext BuildContext(string value)
    {
        var routeValues = new RouteValueDictionary();
        if (value is not null)
        {
            routeValues["showId"] = value;
        }

        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor(),
            new ModelStateDictionary());

        var metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(Guid));

        return new DefaultModelBindingContext
        {
            ActionContext = actionContext,
            ModelState = new ModelStateDictionary(),
            ModelMetadata = metadata,
            ModelName = "showId",
            BindingSource = BindingSource.Path,
            IsTopLevelObject = true,
            ValueProvider = new RouteValueProvider(BindingSource.Path, routeValues),
        };
    }

    [Test]
    public async Task BindAsync_CanonicalBase62Route_BindsOriginalGuid()
    {
        // Arrange
        var binder = new ApiIdRouteModelBinder();
        var context = BuildContext(SampleBase62);

        // Act
        await binder.BindModelAsync(context);

        // Assert
        context.Result.IsModelSet.Should().BeTrue();
        context.Result.Model.Should().Be(Sample);
    }

    [Test]
    public async Task BindAsync_LegacyDashedGuid_FailsClosedWithModelError()
    {
        // Arrange
        var binder = new ApiIdRouteModelBinder();
        var context = BuildContext("E9C1FA23-55AF-4711-8E34-3B31E2A75533");

        // Act
        await binder.BindModelAsync(context);

        // Assert
        context.Result.IsModelSet.Should().BeFalse();
        context.ModelState["showId"]!.Errors.Should().NotBeEmpty();
    }

    [Test]
    public async Task BindAsync_MissingRouteValue_DoesNotBind()
    {
        // Arrange
        var binder = new ApiIdRouteModelBinder();
        var context = BuildContext(null!);

        // Act
        await binder.BindModelAsync(context);

        // Assert
        context.Result.IsModelSet.Should().BeFalse();
        context.ModelState.IsValid.Should().BeTrue();
    }
}
