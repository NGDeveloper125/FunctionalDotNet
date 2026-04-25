namespace FunctionalDotNet;

/// <summary>
/// The outcome of a computation that either succeeds with a value of type
/// <typeparamref name="TSuccess"/> or fails with a value of type
/// <typeparamref name="TError"/>.
/// </summary>
/// <remarks>
/// <para>
/// Result is the library's tool for railway-oriented programming: expected
/// failure is modelled as a value, not as a thrown exception, so the caller
/// cannot drop it without the code looking visibly wrong.
/// </para>
/// <para>
/// The two cases — <see cref="Success"/> and <see cref="Failure"/> — are
/// declared as nested sealed records of an abstract base whose only
/// constructor is private. That combination prevents any third party from
/// declaring a third case, which is what lets pattern matching against
/// <c>Success</c> and <c>Failure</c> be treated as exhaustive.
/// </para>
/// <para>
/// Type-parameter order follows the F# / Rust convention: success first,
/// error second.
/// </para>
/// </remarks>
public abstract record Result<TSuccess, TError>
{
    private Result() { }

    /// <summary>The success case, carrying a <typeparamref name="TSuccess"/> value.</summary>
    public sealed record Success(TSuccess Value) : Result<TSuccess, TError>;

    /// <summary>The failure case, carrying a <typeparamref name="TError"/> value.</summary>
    public sealed record Failure(TError Error) : Result<TSuccess, TError>;

    /// <summary>
    /// Apply <paramref name="mapper"/> to the success value; pass the failure
    /// case through unchanged. The Result functor's <c>fmap</c>.
    /// </summary>
    public Result<TNext, TError> Map<TNext>(Func<TSuccess, TNext> mapper) => this switch
    {
        Success s => new Result<TNext, TError>.Success(mapper(s.Value)),
        Failure f => new Result<TNext, TError>.Failure(f.Error),
        _ => throw new InvalidOperationException("Unreachable: Result has only Success and Failure cases.")
    };

    /// <summary>
    /// Chain a Result-returning step. If this Result is a <see cref="Failure"/>
    /// the chain short-circuits and <paramref name="binder"/> is never called.
    /// The Result monad's <c>bind</c> (also known as <c>flatMap</c> /
    /// <c>SelectMany</c>).
    /// </summary>
    public Result<TNext, TError> Bind<TNext>(Func<TSuccess, Result<TNext, TError>> binder) => this switch
    {
        Success s => binder(s.Value),
        Failure f => new Result<TNext, TError>.Failure(f.Error),
        _ => throw new InvalidOperationException("Unreachable: Result has only Success and Failure cases.")
    };

    /// <summary>
    /// Reduce a Result to a single value of type <typeparamref name="TOut"/>
    /// by handling both cases. The natural exit point of a Result pipeline.
    /// </summary>
    public TOut Match<TOut>(Func<TSuccess, TOut> onSuccess, Func<TError, TOut> onFailure) => this switch
    {
        Success s => onSuccess(s.Value),
        Failure f => onFailure(f.Error),
        _ => throw new InvalidOperationException("Unreachable: Result has only Success and Failure cases.")
    };
}
