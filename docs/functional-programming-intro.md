# Introduction to Functional Programming

This page lays out the core ideas of functional programming (FP) — what it is, the rules it plays by, and the tools that show up over and over again. It's written with C# in mind, but most of what's here is paradigm-level and applies to any language that supports functional style.

As exercises in this repository grow, each concept below will be cross-linked to concrete examples in code. Where a link is missing, the example hasn't been written yet — consider it an open invitation to contribute one.

---

## Table of contents

- [What is functional programming?](#what-is-functional-programming)
- [The core principles](#the-core-principles)
  - [Pure functions](#pure-functions)
  - [Immutability](#immutability)
  - [Referential transparency](#referential-transparency)
  - [Functions as first-class citizens](#functions-as-first-class-citizens)
  - [Higher-order functions](#higher-order-functions)
  - [Function composition](#function-composition)
  - [Expressions over statements](#expressions-over-statements)
  - [Declarative over imperative](#declarative-over-imperative)
- [Techniques you'll see repeatedly](#techniques-youll-see-repeatedly)
  - [Pattern matching](#pattern-matching)
  - [Recursion](#recursion)
  - [Map, Filter, Fold](#map-filter-fold)
  - [Option / Maybe — modelling absence](#option--maybe--modelling-absence)
  - [Either / Result — modelling failure](#either--result--modelling-failure)
  - [Currying and partial application](#currying-and-partial-application)
  - [Lazy evaluation](#lazy-evaluation)
- [Chaining computations: discriminated unions, monoids, and monads](#chaining-computations-discriminated-unions-monoids-and-monads)
  - [Discriminated unions](#discriminated-unions)
  - [Monoids: things you can combine](#monoids-things-you-can-combine)
  - [Monads: chaining in a context](#monads-chaining-in-a-context)
  - [Chaining in practice: Bind, LINQ, and railway-oriented programming](#chaining-in-practice-bind-linq-and-railway-oriented-programming)
  - [Applicative validation: monoids and monads working together](#applicative-validation-monoids-and-monads-working-together)
- [Functional programming in C#](#functional-programming-in-c)
- [Further reading](#further-reading)

---

## What is functional programming?

Functional programming is a way of writing software in which computation is expressed as the **evaluation of functions** rather than as a sequence of steps that change state. A program is built by composing small, well-behaved functions into larger ones, rather than by issuing commands that mutate variables and objects over time.

Two useful contrasts:

- **Imperative programming** answers *"how do I get there?"* — a recipe of steps.
- **Functional programming** answers *"what is the result?"* — a description of the answer in terms of inputs.

FP doesn't forbid state or I/O — real programs need both. What it insists on is keeping the two things separate: a core of pure, predictable logic, with mutation and side effects pushed to the edges.

## The core principles

### Pure functions

A **pure function** is one whose output depends only on its inputs, and which produces no observable side effects. Given the same inputs, it always returns the same output. It does not read a clock, touch a file, throw exceptions for control flow, or mutate anything its caller can see.

Pure functions are the building block of FP. They are trivial to reason about, trivial to test (no setup, no mocks), and they compose without surprises.

> **Example in code:** [PrimeNumbersExercise.IsPrime](../FunctionalCodingExercises/PrimeNumbersExercise.cs) — a pure function from `int` to `bool`.

### Immutability

An **immutable** value cannot be changed after it has been created. Instead of modifying a value, you produce a new one. Immutability is what makes pure functions possible in practice: if nothing can change underneath you, you don't have to worry about *when* or *in what order* things happen.

In C#, immutability shows up in `record` types, `readonly` fields, `init`-only setters, and the `with` expression for non-destructive updates.

### Referential transparency

An expression is **referentially transparent** if it can be replaced by its value without changing the program's behaviour. `add(2, 3)` is referentially transparent if it always produces `5` with no other effects — you could delete the call and hard-code `5` and nothing else would change.

This property is what makes functional code so easy to refactor: you can move expressions around, inline them, extract them, cache them, or run them in parallel, all without fear.

### Functions as first-class citizens

In FP, a function is a value like any other. You can:

- store a function in a variable
- pass a function as an argument
- return a function from another function
- build collections of functions

In C#, this is what `Func<,>`, `Action<>`, and lambda expressions give you.

### Higher-order functions

A **higher-order function** is a function that takes another function as an argument, returns one, or both. `Where`, `Select`, and `Aggregate` in LINQ are all higher-order — they accept a function describing *what* to filter, transform, or combine, and take care of the rest.

Higher-order functions are how FP achieves reuse: instead of writing a loop for every operation, you write one loop (`fold`) and parameterise the per-element behaviour.

> **Example in code:** [FizzBuzzExercise.FizzBuzz](../FunctionalCodingExercises/FizzBuzzExercise.cs) passes a lambda to `ForEach`; [PrimeNumbersExercise.PrintPrimeNumbers](../FunctionalCodingExercises/PrimeNumbersExercise.cs) passes `IsPrime` to `Where`.

### Function composition

**Composition** is combining two functions into a new one: given `f: A → B` and `g: B → C`, the composition `g ∘ f` is a function `A → C`. You chain small functions together to build bigger ones, the same way you'd pipe data through a series of transformations.

LINQ method chains (`xs.Where(...).Select(...).Aggregate(...)`) are composition in the small. Libraries like LaYumba.Functional let you compose bare functions more directly.

### Expressions over statements

A **statement** performs an action (`if (x > 0) y = 1; else y = -1;`). An **expression** produces a value (`var y = x > 0 ? 1 : -1;`). Functional code prefers expressions: every piece of syntax gives back a value, and those values compose.

C# has become much friendlier to this style over the years. Expression-bodied members, switch expressions, target-typed `new`, pattern matching, and ternaries all push you toward expression-style code.

> **Example in code:** [FizzBuzzExercise.FizzBuzz](../FunctionalCodingExercises/FizzBuzzExercise.cs) uses nested ternaries instead of `if`/`else`; [PrimeNumbersExercise.IsPrime](../FunctionalCodingExercises/PrimeNumbersExercise.cs) uses a switch expression.

### Declarative over imperative

**Declarative** code says *what* the answer is; **imperative** code says *how* to compute it step by step. `numbers.Where(IsPrime)` is declarative — it's a description. A `for` loop that walks the list, tests each element, and appends matches to an accumulator is imperative — it's a procedure.

Declarative code tends to be shorter, closer to the problem description, and easier to change because it doesn't fix the how.

## Techniques you'll see repeatedly

### Pattern matching

**Pattern matching** is a way to inspect the structure of a value and branch based on it, as a single expression. It replaces long chains of `if`/`else if` and `is` checks with something that reads like a mathematical case analysis.

C# supports constant patterns, type patterns, property patterns, relational patterns, list patterns, and combinations of these via `switch` expressions.

> **Example in code:** [PrimeNumbersExercise.IsPrime](../FunctionalCodingExercises/PrimeNumbersExercise.cs) uses a switch expression with relational and constant patterns.

### Recursion

In a paradigm that discourages mutation, **recursion** takes the place of the mutable loop. A recursive function solves a problem by solving a smaller version of the same problem and combining the result.

Recursion is natural for tree-shaped data and for problems defined inductively (factorial, Fibonacci, tree traversal). In C# you have to be mindful of stack depth — the language does not guarantee tail-call optimisation — which is one reason higher-order functions like `fold` are often preferred for linear data.

### Map, Filter, Fold

These three higher-order functions are the workhorses of functional programming over collections:

- **Map** (`Select` in LINQ) — apply a function to each element, producing a new collection.
- **Filter** (`Where` in LINQ) — keep only the elements matching a predicate.
- **Fold** (`Aggregate` in LINQ) — reduce a collection to a single value by combining elements.

Almost any loop you would write imperatively can be rewritten as a composition of these three. Fold in particular is the most general — both `map` and `filter` can be implemented in terms of it.

### Option / Maybe — modelling absence

`null` is famously problematic: it's a value of every reference type that isn't really a value of that type, and it leaks through signatures silently. FP replaces it with an explicit **`Option<T>`** (also called `Maybe<T>`): a value is either `Some(x)` or `None`. The type system forces the caller to handle both cases.

LaYumba.Functional provides `Option<T>` along with the operations that let you chain computations over it without unwrapping at every step. See [Chaining computations](#chaining-computations-discriminated-unions-monoids-and-monads) for how that chaining actually works.

### Either / Result — modelling failure

Where `Option` models "value or nothing", **`Either<L, R>`** (often called `Result<TError, TOk>`) models "value or reason it isn't there". It replaces exceptions as a means of *expected* failure — parsing, validation, lookups — while leaving exceptions for *unexpected* failures (out of memory, bugs).

Chains of `Either` operations are the basis of **railway-oriented programming**, covered under [Chaining computations](#chaining-computations-discriminated-unions-monoids-and-monads).

### Currying and partial application

**Currying** transforms a function of many arguments into a chain of functions of one argument each: `(a, b) → c` becomes `a → b → c`. **Partial application** is supplying some arguments now and the rest later, producing a new function.

Both techniques make it easy to specialise general functions into more specific ones without writing a wrapper each time.

### Lazy evaluation

A **lazy** value is not computed until it's actually needed. In C#, `IEnumerable<T>` is lazy by default — a LINQ query describes a computation but doesn't execute it until you iterate. Lazy evaluation lets you work with infinite sequences, skip work you don't need, and compose pipelines without materialising intermediate collections.

## Chaining computations: discriminated unions, monoids, and monads

Everything so far has been about individual principles and techniques. The step from *"I can write a pure function"* to *"I can build a whole program functionally"* happens here: you need a way to **combine** computations so that the result of one flows into the next — cleanly, and even when some of them might fail, return nothing, or produce many values.

Much of our future work in this repository will lean on these ideas, so it's worth taking them slowly.

### Discriminated unions

A **discriminated union** (also called a *sum type*, *tagged union*, or *variant type*) is a type whose value is exactly one of a fixed set of *cases*, where each case may carry different data. A traffic light is a discriminated union of `Red`, `Amber`, and `Green`. An `Option<int>` is a discriminated union of `Some(int)` and `None`. A parse result is a discriminated union of `Success(tree)` and `Failure(error)`.

Discriminated unions matter for FP because they make case analysis **explicit and exhaustive**. When you get an `Option<int>`, the compiler (or library) expects you to handle both `Some` and `None` — no silent `null` slipping through. When a function returns `Either<ParseError, Ast>`, the caller cannot ignore the error case without the code looking visibly wrong.

C# does not have first-class discriminated unions yet, but several patterns emulate them:

- **Class hierarchies** — an `abstract record Shape` with concrete `Circle`, `Square`, `Triangle` subtypes, exhaustively matched with a switch expression.
- **Library types** — LaYumba.Functional's `Option<T>` and `Either<L, R>` expose their cases through factory methods and pattern-matching helpers.
- **Nullable reference types** — a partial substitute for `Option<T>`, enforced by the compiler's flow analysis rather than the type system proper.

These types are the *ingredients*. The interesting question is how you use them without writing a `switch` every two lines. That question has two classic answers: **monoids** and **monads**.

### Monoids: things you can combine

A **monoid** is any type equipped with:

1. a binary operation `combine : T → T → T` that is **associative**, i.e. `(a ⊕ b) ⊕ c = a ⊕ (b ⊕ c)`, and
2. an **identity** element `empty : T` such that `a ⊕ empty = empty ⊕ a = a`.

That's it. No magic, no category theory required. If you have ever added numbers, concatenated strings, or appended lists, you have used a monoid:

| Type                    | Combine  | Identity |
|-------------------------|----------|----------|
| `int` (addition)        | `+`      | `0`      |
| `int` (multiplication)  | `*`      | `1`      |
| `string`                | `+`      | `""`     |
| `List<T>`               | `Concat` | `[]`     |
| `bool` (AND)            | `&&`     | `true`   |
| `bool` (OR)             | `\|\|`   | `false`  |

Why does this abstract pattern matter? Because any monoid can be **folded** over a collection — you start from the identity and combine elements one at a time. `Sum()`, `Concat()`, `All()`, `Any()`, and the general `Aggregate()` are all the same idea applied to different monoids. And because the operation is associative, folds can be reordered or parallelised without changing the result.

In practice, monoids show up whenever you need to *accumulate* something:

- collecting log entries across a pipeline of functions,
- merging configuration layers (defaults + environment + user overrides),
- combining partial results from parallel work,
- gathering validation errors across every field of a form (more on this below).

### Monads: chaining in a context

Monoids compose *values*. Monads compose *computations*. A **monad** is a type constructor `M<T>` — a "generic wrapper" — with two operations:

1. **Return** (also called `Pure` or `Unit`) — lifts a plain value into the wrapper:
   `T → M<T>`
2. **Bind** (also called `FlatMap`, `SelectMany`, or `>>=`) — chains a wrapped value with a function that produces another wrapped value:
   `M<T> → (T → M<U>) → M<U>`

The type `M<T>` represents a **computational context**: some extra structure sitting around the value. What the context *means* depends on which monad you are looking at:

| Monad             | What the context means                               |
|-------------------|------------------------------------------------------|
| `Option<T>`       | the value might be missing                           |
| `Either<L, T>`    | the computation might have failed, with reason `L`   |
| `IEnumerable<T>`  | there may be zero, one, or many values               |
| `Task<T>`         | the value will arrive asynchronously                 |
| `Try<T>`          | the computation might have thrown an exception       |

`Bind` is the load-bearing operation. Given an `Option<int>` and a function `int → Option<string>`, `Bind` feeds the integer into the function *only if the option actually has a value*, and propagates `None` through otherwise. The caller writes linear code; the monad handles the branching.

That is the pattern behind every monad: **Bind knows how to propagate the context so you don't have to**.

Every well-behaved monad obeys three **laws**:

- **Left identity** — `Return(x).Bind(f) == f(x)`
- **Right identity** — `m.Bind(Return) == m`
- **Associativity** — `m.Bind(f).Bind(g) == m.Bind(x => f(x).Bind(g))`

You don't have to memorise these; most of the time you just use the monad and trust the library. But the laws are what makes Bind-chains safe to refactor as freely as arithmetic expressions — you can regroup, extract, inline, and the meaning does not change.

### Chaining in practice: Bind, LINQ, and railway-oriented programming

Here's the payoff. Imagine a pipeline that parses a user's id, looks them up in a database, and returns their postal code. Each step can fail.

Without monads (imperative-ish, null-based):

```csharp
int? id = TryParseId(input);
if (id == null) return null;

User? user = FindUser(id.Value);
if (user == null) return null;

string? postcode = user.Postcode;
return postcode;
```

With `Option<T>` and `Bind`:

```csharp
Option<string> postcode =
    TryParseId(input)
        .Bind(FindUser)
        .Bind(u => u.Postcode);
```

Or, because **LINQ's `SelectMany` *is* `Bind`**, the C# compiler can desugar query syntax against any type that implements it — making LINQ a general-purpose "do-notation" for monads:

```csharp
Option<string> postcode =
    from id       in TryParseId(input)
    from user     in FindUser(id)
    from postcode in user.Postcode
    select postcode;
```

In every version, the logic is the same. In the monadic versions, **the plumbing is gone**: no `null` checks, no early returns, no nested conditionals. The `Option` monad handles absence; the author only writes the happy path.

The same pattern with `Either<Error, T>` is called **railway-oriented programming**: success flows straight down the happy track, and the first `Left(error)` shunts the computation onto the error track, short-circuiting everything that follows. You get exception-style short-circuit semantics with explicit, type-checked error values — no try/catch, no nulls, no silent failures.

Swap the monad and the shape of the code stays the same while the meaning changes: the same Bind-chain over `Task<T>` becomes an async pipeline; over `IEnumerable<T>` it becomes a cross-product; over `Try<T>` it becomes exception-safe code. **This is the real reason monads are worth the conceptual cost**: one pattern, many contexts.

### Applicative validation: monoids and monads working together

One last pattern, because it shows both ideas doing useful work side by side: **applicative validation**.

With a monadic `Either`, the first error wins and the rest of the computation is skipped — which is the *wrong* behaviour for form validation, where you want to show the user every mistake at once, not just the first one.

The fix is to treat the error side as a **monoid** (typically a list of error messages) and combine results **applicatively** rather than monadically. Each field is validated independently; each failure contributes its errors to a growing list; only if every field succeeds does the combined result succeed. LaYumba.Functional's `Validation<T>` is exactly this.

The takeaway: monoids and monads are not competing ideas. Different situations want one or the other, and sophisticated types like `Validation<T>` use **a monoid (for errors) inside a context (for success or failure)** to get the best of both. We'll build one of these from scratch in a later exercise.

## Functional programming in C#

C# is not a purely functional language. It has mutation, inheritance, `null`, and exceptions baked in. But over time it has absorbed enough functional features that a consistent functional style is practical:

- `record` types and `init`-only properties for immutability
- Pattern matching and switch expressions
- Expression-bodied members
- `Func<>` / `Action<>` / lambdas for first-class functions
- LINQ for declarative collection processing
- Nullable reference types for explicit absence (a partial alternative to `Option`)

What the language still doesn't give you — discriminated unions, proper `Option` / `Either`, higher-kinded types, pipe operators, guaranteed tail calls — is what libraries like [LaYumba.Functional](https://github.com/la-yumba/functional-csharp-code) fill in.

The practical upshot: idiomatic functional C# is a *hybrid*. You keep the parts of the language that pull their weight (LINQ, records, pattern matching), you bring in a library for the missing pieces, and you push mutation and I/O to the edges of your program so the core stays pure.

## Further reading

- Enrico Buonanno — *Functional Programming in C#* (Manning). The definitive book on the topic and the source of the LaYumba.Functional library.
- [LaYumba.Functional on GitHub](https://github.com/la-yumba/functional-csharp-code) — the library used in this repository.
- Scott Wlaschin — *Domain Modeling Made Functional* (Pragmatic Bookshelf). F#-focused, but the modelling ideas transfer directly.
- [F# for Fun and Profit](https://fsharpforfunandprofit.com/) — a deep, approachable online resource on FP concepts, Scott Wlaschin again.
