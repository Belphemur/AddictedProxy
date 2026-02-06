using Microsoft.AspNetCore.Http.HttpResults;

namespace AddictedProxy.Utils;

public static class ResultsExtensions
{
    /// <summary>
    /// Match on a Results&lt;Ok&lt;T&gt;, NotFound&gt; to handle both cases
    /// </summary>
    /// <remarks>
    /// The Ok&lt;T&gt; value is guaranteed to be non-null by the ASP.NET Core framework.
    /// </remarks>
    public static TResult Match<T, TResult>(
        this Results<Ok<T>, NotFound> result,
        Func<T, TResult> onOk,
        Func<TResult> onNotFound)
    {
        return result.Result switch
        {
            Ok<T> ok => onOk(ok.Value!), // Value is guaranteed non-null by Ok<T>
            NotFound => onNotFound(),
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Match on a Results&lt;Ok&lt;T&gt;, NotFound&gt; to handle both cases asynchronously
    /// </summary>
    /// <remarks>
    /// The Ok&lt;T&gt; value is guaranteed to be non-null by the ASP.NET Core framework.
    /// </remarks>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Results<Ok<T>, NotFound> result,
        Func<T, Task<TResult>> onOk,
        Func<TResult> onNotFound)
    {
        return result.Result switch
        {
            Ok<T> ok => await onOk(ok.Value!), // Value is guaranteed non-null by Ok<T>
            NotFound => onNotFound(),
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Match on a Results&lt;Ok&lt;T&gt;, BadRequest&gt; to handle both cases
    /// </summary>
    /// <remarks>
    /// The Ok&lt;T&gt; value is guaranteed to be non-null by the ASP.NET Core framework.
    /// </remarks>
    public static TResult Match<T, TResult>(
        this Results<Ok<T>, BadRequest> result,
        Func<T, TResult> onOk,
        Func<TResult> onBadRequest)
    {
        return result.Result switch
        {
            Ok<T> ok => onOk(ok.Value!), // Value is guaranteed non-null by Ok<T>
            BadRequest => onBadRequest(),
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Match on a Results&lt;Ok&lt;T&gt;, BadRequest&gt; to handle both cases asynchronously
    /// </summary>
    /// <remarks>
    /// The Ok&lt;T&gt; value is guaranteed to be non-null by the ASP.NET Core framework.
    /// </remarks>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Results<Ok<T>, BadRequest> result,
        Func<T, Task<TResult>> onOk,
        Func<TResult> onBadRequest)
    {
        return result.Result switch
        {
            Ok<T> ok => await onOk(ok.Value!), // Value is guaranteed non-null by Ok<T>
            BadRequest => onBadRequest(),
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Match on a Results&lt;Ok&lt;T&gt;, BadRequest&gt; to handle both cases asynchronously with async handlers
    /// </summary>
    /// <remarks>
    /// The Ok&lt;T&gt; value is guaranteed to be non-null by the ASP.NET Core framework.
    /// </remarks>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Results<Ok<T>, BadRequest> result,
        Func<T, Task<TResult>> onOk,
        Func<Task<TResult>> onBadRequest)
    {
        return result.Result switch
        {
            Ok<T> ok => await onOk(ok.Value!), // Value is guaranteed non-null by Ok<T>
            BadRequest => await onBadRequest(),
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Map the Ok value to another type while preserving the error case
    /// </summary>
    /// <remarks>
    /// The Ok&lt;TIn&gt; value is guaranteed to be non-null by the ASP.NET Core framework.
    /// </remarks>
    public static async Task<Results<Ok<TOut>, NotFound>> MapAsync<TIn, TOut>(
        this Results<Ok<TIn>, NotFound> result,
        Func<TIn, Task<TOut>> mapper)
    {
        return result.Result switch
        {
            Ok<TIn> ok => TypedResults.Ok(await mapper(ok.Value!)), // Value is guaranteed non-null by Ok<TIn>
            NotFound notFound => notFound,
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Bind operation that chains Results transformations
    /// </summary>
    /// <remarks>
    /// The Ok&lt;TIn&gt; value is guaranteed to be non-null by the ASP.NET Core framework.
    /// </remarks>
    public static async Task<Results<Ok<TOut>, NotFound>> BindAsync<TIn, TOut>(
        this Results<Ok<TIn>, NotFound> result,
        Func<TIn, Task<Results<Ok<TOut>, NotFound>>> binder)
    {
        return result.Result switch
        {
            Ok<TIn> ok => await binder(ok.Value!), // Value is guaranteed non-null by Ok<TIn>
            NotFound notFound => notFound,
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Bind operation that chains Results transformations with BadRequest
    /// </summary>
    /// <remarks>
    /// The Ok&lt;TIn&gt; value is guaranteed to be non-null by the ASP.NET Core framework.
    /// </remarks>
    public static async Task<Results<Ok<TOut>, BadRequest>> BindAsync<TIn, TOut>(
        this Results<Ok<TIn>, NotFound> result,
        Func<TIn, Task<Results<Ok<TOut>, BadRequest>>> binder,
        Func<Results<Ok<TOut>, BadRequest>> onNotFound)
    {
        return result.Result switch
        {
            Ok<TIn> ok => await binder(ok.Value!), // Value is guaranteed non-null by Ok<TIn>
            NotFound => onNotFound(),
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }
}
