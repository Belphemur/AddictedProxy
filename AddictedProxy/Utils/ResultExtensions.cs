using Ardalis.Result;

namespace AddictedProxy.Utils;

public static class ResultExtensions
{
    public static async Task<Result<TDestination>> MapAsync<TSource, TDestination>(
        this Result<TSource> result,
        Func<TSource, Task<Result<TDestination>>> func
    )
    {
        return result.Status switch
        {
            ResultStatus.Ok => await func((TSource)result),
            ResultStatus.Error => Result<TDestination>.CriticalError(result.Errors.ToArray<string>()),
            ResultStatus.Forbidden => Result<TDestination>.Forbidden(),
            ResultStatus.Unauthorized => Result<TDestination>.Unauthorized(),
            ResultStatus.Invalid => Result<TDestination>.Invalid(result.ValidationErrors.ToArray()),
            ResultStatus.NotFound => Result<TDestination>.NotFound(),
            ResultStatus.Conflict => Result<TDestination>.Conflict(result.Errors.ToArray<string>()),
            ResultStatus.CriticalError => Result<TDestination>.CriticalError(result.Errors.ToArray<string>()),
            ResultStatus.Unavailable => Result<TDestination>.Unavailable(),
            _ => throw new NotSupportedException()
        };
    }

    public static async Task<Result<TDestination>> MapAsync<TSource, TDestination>(
        this Task<Result<TSource>> taskResult,
        Func<TSource, Task<Result<TDestination>>> func
    )
    {
        var result = await taskResult;
        return result.Status switch
        {
            ResultStatus.Ok => await func((TSource)result),
            ResultStatus.Error => Result<TDestination>.CriticalError(result.Errors.ToArray<string>()),
            ResultStatus.Forbidden => Result<TDestination>.Forbidden(),
            ResultStatus.Unauthorized => Result<TDestination>.Unauthorized(),
            ResultStatus.Invalid => Result<TDestination>.Invalid(result.ValidationErrors.ToArray()),
            ResultStatus.NotFound => Result<TDestination>.NotFound(),
            ResultStatus.Conflict => Result<TDestination>.Conflict(result.Errors.ToArray<string>()),
            ResultStatus.CriticalError => Result<TDestination>.CriticalError(result.Errors.ToArray<string>()),
            ResultStatus.Unavailable => Result<TDestination>.Unavailable(),
            _ => throw new NotSupportedException()
        };
    }

    public static async Task<Result<TDestination>> MapAsync<TSource, TDestination>(
        this Task<Result<TSource>> taskResult,
        Func<TSource, TDestination> func
    )
    {
        var result = await taskResult;
        return result.Status switch
        {
            ResultStatus.Ok => func((TSource)result),
            ResultStatus.Error => Result<TDestination>.CriticalError(result.Errors.ToArray<string>()),
            ResultStatus.Forbidden => Result<TDestination>.Forbidden(),
            ResultStatus.Unauthorized => Result<TDestination>.Unauthorized(),
            ResultStatus.Invalid => Result<TDestination>.Invalid(result.ValidationErrors.ToArray()),
            ResultStatus.NotFound => Result<TDestination>.NotFound(),
            ResultStatus.Conflict => Result<TDestination>.Conflict(result.Errors.ToArray<string>()),
            ResultStatus.CriticalError => Result<TDestination>.CriticalError(result.Errors.ToArray<string>()),
            ResultStatus.Unavailable => Result<TDestination>.Unavailable(),
            _ => throw new NotSupportedException()
        };
    }
}