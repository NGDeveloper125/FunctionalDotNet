# FunctionalDotNet

The library — what it is, why it exists, what it will contain, and how to use it.

This document grows as the library does. Right now the public surface is empty: this is the design preface, the "what we are about to build and why". Each type will get its own section once it lands, with the rationale, the API, and worked examples.

For the paradigm-level concepts the library is built on — pure functions, immutability, monoids, monads — see the [introduction to functional programming](functional-programming-intro.md). This doc assumes that vocabulary.

## What the library is

`FunctionalDotNet` is a class library that fills in the functional building blocks C# doesn't ship with: explicit absence (`Option<T>`), explicit failure (`Either<L, R>`), accumulating validation (`Validation<T>`), function composition helpers, and the standard set of operations (`Map`, `Bind`, `Match`, …) that make those types pleasant to use.

The library is plain C# and targets `net10.0`. It has no runtime dependencies on third-party packages and is intended to be published as a NuGet package once the API is stable.

## What the library is not

- **Not a port.** Other libraries in this space (`LaYumba.Functional`, `LanguageExt`) are excellent and well-established. This one is not trying to compete with them; it exists to be built up from first principles in the open, with the reasoning visible.
- **Not a framework.** It is a small set of orthogonal types and combinators. It does not impose an application architecture, a DI story, or a way of doing I/O.
- **Not a replacement for the BCL.** It complements LINQ, records, pattern matching, and nullable reference types. Where the BCL already does the job (and it often does), the library defers.

## Design principles

These are the rules we hold ourselves to when adding to the library.

1. **Every type earns its place.** A new type goes in only when there is a use case the existing primitives handle clumsily. No speculative additions.
2. **The why is documented with the what.** Each type's section in this document explains the problem it solves before it shows the API.
3. **Honour the laws.** Monadic types satisfy the monad laws; monoidal types satisfy associativity and identity. Where laws are non-obvious we'll spell them out and back them with property tests.
4. **Composable, not clever.** Small operations that combine cleanly beat large operations that try to do everything.
5. **C#-native ergonomics.** Where a feature has an idiomatic C# spelling (LINQ query syntax for `Bind`, deconstruction for tuples, pattern matching for case analysis), the library supports it.
6. **No exceptions for expected failure.** `Either` and `Validation` are how the library signals failure. Exceptions are reserved for genuinely unexpected conditions (bugs, resource exhaustion).
7. **Nullable reference types on, always.** `Option<T>` and `null` are different tools. Both are in scope; the library does not pretend `null` doesn't exist, but it never returns it from its own API.

## Roadmap of types

The order is roughly the order we expect to build them. Each will get a full section here once implemented.

- `Option<T>` — explicit absence.
- `Either<L, R>` — explicit failure with a reason.
- `Validation<T>` — accumulating, applicative validation.
- Function composition helpers — `Compose`, `Pipe`, currying, partial application.
- `Try<T>` — exception-safe computation.
- `Reader<E, T>` / `State<S, T>` — if and when example projects motivate them.

Anything below the line of "if and when example projects motivate them" is provisional. The library is grown by demand from the example projects, not from a feature checklist.

## Public API

*(Empty for now. Each type will get its own subsection here once it lands, in this shape:*

> **Why it exists** — the problem it solves.
> **Type and constructors** — how you make one.
> **Core operations** — `Map`, `Bind`, `Match`, etc., with signatures.
> **Examples** — short, realistic snippets.
> **Laws** — the algebraic properties the type satisfies, where applicable.

*)*

## Versioning and stability

The library follows semantic versioning. Until 1.0 the API is allowed to break between minor versions; the changelog will say so explicitly when it does. Breaking changes after 1.0 will only happen on major-version bumps.

## Testing

Every public type has unit tests in [`FunctionalDotNet.Tests`](../FunctionalDotNet.Tests/). For monadic and monoidal types the tests include law checks (left identity, right identity, associativity for monads; associativity and identity for monoids), so a refactor that breaks an algebraic guarantee fails loudly.

## Using the library

Once published, you'll be able to install the package with:

```bash
dotnet add package FunctionalDotNet
```

Until then, reference the project directly:

```xml
<ProjectReference Include="..\FunctionalDotNet\FunctionalDotNet.csproj" />
```

Usage examples for each type live alongside that type's section above, and the example projects in this repository show the library in realistic application settings.
