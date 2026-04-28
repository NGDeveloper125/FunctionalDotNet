namespace FunctionalDotNet;

/// <summary>
/// A value that may or may not be present. The library's tool for
/// <em>explicit absence</em>.
/// </summary>
/// <remarks>
/// <para>
/// Where C# lets <c>null</c> silently leak through any reference type,
/// Option puts "missing" into the type system: a caller cannot reach the
/// inner value without first acknowledging that <see cref="None"/> exists.
/// </para>
/// <para>
/// The two cases — <see cref="Some"/> and <see cref="None"/> — are
/// declared as nested sealed records of an abstract base whose only
/// constructor is private. That combination prevents any third party from
/// introducing a third case, which is what lets pattern matching against
/// <c>Some</c> and <c>None</c> be treated as exhaustive.
/// </para>
/// <para>
/// <see cref="None"/> carries no data, because absence has no further
/// explanation. If the failure has a reason that the caller needs to know,
/// reach for <see cref="Result{TSuccess, TError}"/> instead — or use
/// <see cref="ToResult{TError}"/> to lift this Option onto the Result
/// track at the boundary where the reason becomes known.
/// </para>
/// </remarks>
public abstract record Option<T>
{
    private Option() { }

    /// <summary>The present case, carrying a value of type <typeparamref name="T"/>.</summary>
    public sealed record Some(T Value) : Option<T>;

    /// <summary>The absent case. Carries no data — absence is the whole story.</summary>
    public sealed record None() : Option<T>;

    /// <summary>
    /// Apply <paramref name="mapper"/> to the value if it is present; pass
    /// <see cref="None"/> through unchanged. Option's <c>fmap</c>.
    /// </summary>
    public Option<TNext> Map<TNext>(Func<T, TNext> mapper) => this switch
    {
        Some s => new Option<TNext>.Some(mapper(s.Value)),
        None => new Option<TNext>.None(),
        _ => throw new InvalidOperationException("Unreachable: Option has only Some and None cases.")
    };

    /// <summary>
    /// Chain an Option-returning step. If this Option is <see cref="None"/>
    /// the chain short-circuits and <paramref name="binder"/> is never
    /// called. Option's <c>bind</c> (also known as <c>flatMap</c> /
    /// <c>SelectMany</c>).
    /// </summary>
    public Option<TNext> Bind<TNext>(Func<T, Option<TNext>> binder) => this switch
    {
        Some s => binder(s.Value),
        None => new Option<TNext>.None(),
        _ => throw new InvalidOperationException("Unreachable: Option has only Some and None cases.")
    };

    /// <summary>
    /// Reduce an Option to a single value of type <typeparamref name="TOut"/>
    /// by handling both cases. The natural exit point of an Option pipeline.
    /// </summary>
    public TOut Match<TOut>(Func<T, TOut> onSome, Func<TOut> onNone) => this switch
    {
        Some s => onSome(s.Value),
        None => onNone(),
        _ => throw new InvalidOperationException("Unreachable: Option has only Some and None cases.")
    };

    /// <summary>
    /// Promote this Option onto the Result track by attaching
    /// <paramref name="error"/> to the <see cref="None"/> case. The
    /// canonical boundary between an Option-returning lookup and the
    /// Result-returning layer above it: the lookup speaks the smallest
    /// honest type, the caller — who knows the context — provides the
    /// reason.
    /// </summary>
    public Result<T, TError> ToResult<TError>(TError error) => this switch
    {
        Some s => new Result<T, TError>.Success(s.Value),
        None => new Result<T, TError>.Failure(error),
        _ => throw new InvalidOperationException("Unreachable: Option has only Some and None cases.")
    };
}
