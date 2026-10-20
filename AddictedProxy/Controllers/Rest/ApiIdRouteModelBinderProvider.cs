using System.Threading.Tasks;
using AddictedProxy.Model.Api;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AddictedProxy.Controllers.Rest;

/// <summary>
/// Converts path-bound <see cref="Guid"/> action parameters from the canonical
/// base62 API encoding (see <see cref="Base62Id"/>), so that the
/// <c>{showId:apiId}</c> route policy can validate URLs without forcing
/// controllers/services off their <see cref="Guid"/> signatures.
///
/// Selected only when the routing value is Path-binding bound; invalid values
/// add a model-state error (fail closed → 400).
/// </summary>
public class ApiIdRouteModelBinder : IModelBinder
{
    /// <inheritdoc />
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        if (!Base62Id.TryDecode(valueResult.FirstValue!, out var guid))
        {
            bindingContext.ModelState.TryAddModelError(
                bindingContext.ModelName,
                $"The value '{valueResult.FirstValue}' is not a valid API identifier (expected {Base62Id.Length} base62 characters).");
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(guid);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Provider routing path-bound <see cref="Guid"/> parameters to
/// <see cref="ApiIdRouteModelBinder"/>. One funnel for the whole repository:
/// every externally addressable Guid is an API id by definition, so the rule
/// qualifies on the binding source, not on per-parameter attributes.
/// </summary>
public class ApiIdRouteModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.ModelType == typeof(Guid)
               && context.BindingInfo?.BindingSource == BindingSource.Path
            ? new ApiIdRouteModelBinder()
            : null;
    }
}
