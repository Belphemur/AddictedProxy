using Microsoft.AspNetCore.Http.HttpResults;

namespace AddictedProxy.Utils;

public static class ResultsExtensions
{
    /// <summary>
    /// Match on a Results&lt;Ok&lt;T&gt;, NotFound&gt; to handle both cases
    /// </summary>
    public static TResult Match<T, TResult>(
        this Results<Ok<T>, NotFound> result,
        Func<T, TResult> onOk,
        Func<TResult> onNotFound)
    {
        return result.Result switch
        {
            Ok<T> ok => onOk(ok.Value!),
            NotFound => onNotFound(),
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Match on a Results&lt;Ok&lt;T&gt;, NotFound&gt; to handle both cases asynchronously
    /// </summary>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Results<Ok<T>, NotFound> result,
        Func<T, Task<TResult>> onOk,
        Func<TResult> onNotFound)
    {
        return result.Result switch
        {
            Ok<T> ok => await onOk(ok.Value!),
            NotFound => onNotFound(),
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Match on a Results&lt;Ok&lt;T&gt;, BadRequest&gt; to handle both cases
    /// </summary>
    public static TResult Match<T, TResult>(
        this Results<Ok<T>, BadRequest> result,
        Func<T, TResult> onOk,
        Func<TResult> onBadRequest)
    {
        return result.Result switch
        {
            Ok<T> ok => onOk(ok.Value!),
            BadRequest => onBadRequest(),
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Match on a Results&lt;Ok&lt;T&gt;, BadRequest&gt; to handle both cases asynchronously
    /// </summary>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Results<Ok<T>, BadRequest> result,
        Func<T, Task<TResult>> onOk,
        Func<TResult> onBadRequest)
    {
        return result.Result switch
        {
            Ok<T> ok => await onOk(ok.Value!),
            BadRequest => onBadRequest(),
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Match on a Results&lt;Ok&lt;T&gt;, BadRequest&gt; to handle both cases asynchronously with async handlers
    /// </summary>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Results<Ok<T>, BadRequest> result,
        Func<T, Task<TResult>> onOk,
        Func<Task<TResult>> onBadRequest)
    {
        return result.Result switch
        {
            Ok<T> ok => await onOk(ok.Value!),
            BadRequest => await onBadRequest(),
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Map the Ok value to another type while preserving the error case
    /// </summary>
    public static async Task<Results<Ok<TOut>, NotFound>> MapAsync<TIn, TOut>(
        this Results<Ok<TIn>, NotFound> result,
        Func<TIn, Task<TOut>> mapper)
    {
        return result.Result switch
        {
            Ok<TIn> ok => TypedResults.Ok(await mapper(ok.Value!)),
            NotFound notFound => notFound,
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Bind operation that chains Results transformations
    /// </summary>
    public static async Task<Results<Ok<TOut>, NotFound>> BindAsync<TIn, TOut>(
        this Results<Ok<TIn>, NotFound> result,
        Func<TIn, Task<Results<Ok<TOut>, NotFound>>> binder)
    {
        return result.Result switch
        {
            Ok<TIn> ok => await binder(ok.Value!),
            NotFound notFound => notFound,
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Bind operation that chains Results transformations with BadRequest
    /// </summary>
    public static async Task<Results<Ok<TOut>, BadRequest>> BindAsync<TIn, TOut>(
        this Results<Ok<TIn>, NotFound> result,
        Func<TIn, Task<Results<Ok<TOut>, BadRequest>>> binder,
        Func<Results<Ok<TOut>, BadRequest>> onNotFound)
    {
        return result.Result switch
        {
            Ok<TIn> ok => await binder(ok.Value!),
            NotFound => onNotFound(),
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }
}
