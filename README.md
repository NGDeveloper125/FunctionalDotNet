# FunctionalDotNet

A functional programming toolkit for C#, plus a growing set of example projects that show what idiomatic functional code looks like across different kinds of .NET applications.

The repository has two faces:

1. **[`FunctionalDotNet`](FunctionalDotNet/)** — a class library that provides the building blocks the standard library doesn't: `Option`, `Either`, `Validation`, function composition helpers, and the rest of the FP toolkit. It is being built from the ground up here, exercise by exercise, and is intended to ship as a NuGet package.
2. **Example projects** — small .NET applications (Web API, Windows Service, desktop app, etc.) that consume `FunctionalDotNet` and demonstrate functional patterns in realistic settings, not just in unit tests.

This is a learning resource and a reference. It's open source, and contributions are welcome.

## Why build our own library?

Most C# functional libraries trace back to Enrico Buonanno's *Functional Programming in C#* and its companion `LaYumba.Functional`. Those are excellent. But building the toolkit ourselves, in the open, has its own value:

- Every type comes with the *reasoning* behind it, not just the API. The library doc explains why `Option<T>` looks the way it does, why `Bind` has the signature it has, why `Validation<T>` accumulates errors instead of short-circuiting.
- It forces a bottom-up understanding of the abstractions instead of treating them as primitives.
- It gives a single, consistent surface area to use across all the example projects.

## Repository layout

```
FunctionalDotNet/              the library (class library, net10.0)
FunctionalDotNet.Tests/        xUnit v3 tests for the library
docs/
    functional-programming-intro.md   paradigm-level introduction to FP
    functionaldotnet-library.md       the library's design notes and usage
FunctionalDotNet.slnx          solution file (slnx format)
```

Example projects (Web API, Windows Service, desktop app, etc.) will be added alongside the library and listed here as they land.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Getting started

```bash
git clone https://github.com/NGDeveloper125/FunctionalDotNet.git
cd FunctionalDotNet
dotnet build
dotnet test --solution FunctionalDotNet.slnx
```

## Documentation

- **[Introduction to functional programming](docs/functional-programming-intro.md)** — the paradigm-level concepts the library is built on: pure functions, immutability, higher-order functions, `Option` / `Either`, monoids and monads.
- **[FunctionalDotNet library](docs/functionaldotnet-library.md)** — the library's own documentation: design decisions, the public API as it grows, and usage examples.

## Status

Early. The solution and project skeletons are in place; the library is empty and we are about to start adding types one at a time, each with its own tests and documentation.

## Contributing

Contributions of all sizes are welcome — a new library type, a new example project, a cleaner implementation, a typo fix, or a suggestion. Open an issue or a pull request.

The only soft rule is: favour clarity over cleverness, and lean into the functional approach rather than "C# with lambdas".
