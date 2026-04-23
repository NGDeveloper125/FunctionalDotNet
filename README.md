# Functional Programming Exercises in C#

A collection of coding exercises solved with a functional programming mindset, written in C#. The goal is to explore how functional concepts translate into a language that is fundamentally object-oriented, and to share both the theoretical ideas and the practical details of implementing them.

This project is open source and meant to be a learning resource, a reference, and a small playground for anyone curious about functional programming in C# — whether you're already comfortable with the paradigm or just starting to look into it.

## What you'll find here

Each exercise lives in its own file and shows one way to tackle a problem from a functional perspective. Some are classic coding puzzles; others explore specific FP concepts or patterns — things like immutability, higher-order functions, function composition, `Option` / `Either` types, pure functions, and expression-based control flow.

The exercises are intentionally kept readable rather than maximally clever. The idea is to learn, not to code-golf.

## Theory

Before diving into code, [the introduction to functional programming](docs/functional-programming-intro.md) lays out the core concepts and rules that the exercises build on — pure functions, immutability, higher-order functions, composition, `Option` / `Either`, and so on. Each concept is (or will be) cross-linked to the exercises that demonstrate it.

## Getting started

Requirements: [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/NGDeveloper125/Functional-Programming-Exercises.git
cd Functional-Programming-Exercises
dotnet run --project FunctionalCodingExercises
```

That launches an interactive menu — pick an exercise, supply input, see the result. To verify everything works end-to-end:

```bash
dotnet test
```

## About LaYumba.Functional

This project uses [LaYumba.Functional](https://github.com/la-yumba/functional-csharp-code), the companion library to Enrico Buonanno's book *Functional Programming in C#*. It provides the building blocks that aren't in the standard library — `Option`, `Either`, `Validation`, function composition helpers, and more — and it's what we lean on whenever an exercise calls for something beyond plain LINQ.

If you want to learn the library (and the functional style it encourages) from the ground up, the book is the canonical resource. The library's own [repository](https://github.com/la-yumba/functional-csharp-code) is a good place to browse the types and see how they're meant to be used.

## Exercises done so far

- [FizzBuzz](FunctionalCodingExercises/FizzBuzzExercise.cs)
- [Prime Numbers](FunctionalCodingExercises/PrimeNumbersExercise.cs)
- [Palindrome](FunctionalCodingExercises/PalindromeExercise.cs)

## Ideas for future exercises

Anyone is welcome to pick one of these and send a PR, or to open an issue suggesting new ones. If there's something on the list you'd rather I tackle, open an issue and say so — same if you want to add a new idea without writing the solution yourself.

### Classic coding puzzles (functional take)
- String reversal without mutable state
- Anagram detector
- Roman numeral converter (both directions)
- Caesar cipher / ROT13
- Fibonacci — naive, memoized, and lazy/infinite-sequence versions
- Sum of digits
- Two-sum
- Valid parentheses
- Merge intervals
- Word frequency counter

### FP fundamentals
- Implementing `Map`, `Filter`, `Fold` from scratch
- Currying and partial application
- Function composition pipelines
- Memoization as a higher-order function
- Recursion vs. fold — when to reach for each
- Pattern matching deep dives (including recursive patterns)

### Working with `Option` / `Maybe`
- Safe parsing of user input
- Dictionary and configuration lookups without nulls
- Chaining optional operations with `Bind` / `Map`
- `Option<T>` vs. nullable reference types — side by side

### Working with `Either` / `Result`
- Error handling without exceptions
- Railway-oriented programming
- Parsing with descriptive errors

### Validation
- Accumulating errors with applicative validation
- Form / DTO validation pipelines

### Immutability and data
- Refactoring a mutable domain model into `record` types
- Implementing a persistent linked list
- Lenses for deep immutable updates

### Algorithms
- Functional merge sort / quicksort
- Tree traversals — map, fold, filter on trees
- Breadth-first and depth-first search
- Lazy infinite sequences (primes, Fibonacci, etc.)

### More ambitious
- A small expression evaluator / calculator
- A JSON parser built from parser combinators
- A mini interpreter for a toy language
- Event sourcing / state reducer pattern
- Free monads — what they are and why they matter

Got a suggestion that isn't here? [Open an issue](https://github.com/NGDeveloper125/Functional-Programming-Exercises/issues).

## Contributing

Contributions of all sizes are welcome — a new exercise, a cleaner solution to an existing one, a typo fix, or just a suggestion. Open an issue or a pull request.

The only soft rule is: favour clarity over cleverness, and lean into the functional approach rather than "C# with lambdas".