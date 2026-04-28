# Discriminated Unions

## What is a discriminated union?

A **discriminated union** (also called a *sum type*, *tagged union*, or
*variant type*) is a type whose value is exactly one of a fixed set of
named *cases*, each of which can carry its own data. Pattern matching on a
discriminated union is exhaustive: the compiler — or the API design — forces
you to acknowledge every case.

The point isn't theoretical. Discriminated unions replace two patterns that
quietly cause bugs in object-oriented C#:

- **`null` as a stand-in for absence.** Every reference type silently
  includes `null`, the runtime never warns you, and the convention "this
  method might return null" lives in the documentation rather than the
  type. A discriminated union makes absence *part of the type*: a function
  that might not return a value returns an `Option<T>`, and there is no
  way to write code that uses the inner value without visibly handling the
  absent case.
- **Exceptions as a control-flow channel for expected failure.** Throwing
  when the user typed a non-number is exception-as-`goto` — the caller has
  to know to wrap the call, the type signature lies about what the
  function can do, and the failure path can't be reasoned about locally.
  A discriminated union like `Result<T, E>` makes the failure a value: it
  is visible in the signature, the caller cannot drop it without the code
  looking visibly wrong, and the reason is data the caller can match on.

C# does not have first-class discriminated unions (as of net10.0). The
library emulates them with an `abstract record` whose constructor is
private and whose only subtypes are nested `sealed record` cases. That
combination prevents any third party from declaring a third case, which is
what makes pattern matching against the cases effectively exhaustive.

---

## `Option<T>` — explicit absence

`Option<T>` represents a value that may or may not be there. It has two
cases:

- **`Some(T value)`** — a value is present.
- **`None`** — there is no value.

`None` carries *no data*. The information content of `None` is "absent",
and that is the whole story. If the failure has a reason the caller needs
to know — a category, a code, a message — `Option` is the wrong type;
reach for `Result` instead.

**Core operations:**

- `Map(T -> U) : Option<T> -> Option<U>` — apply a function to the value
  if there is one; pass `None` through unchanged.
- `Bind(T -> Option<U>) : Option<T> -> Option<U>` — chain another
  Option-returning step; the chain short-circuits on `None`.
- `Match(T -> R, () -> R) : Option<T> -> R` — collapse to a single value
  by handling both cases.
- `ToResult(E error) : Option<T> -> Result<T, E>` — promote an Option
  onto the Result track by attaching a reason for the absence. This is
  the canonical boundary between Option-returning lookups and the
  Result-returning layer above them.

**Example in this repo.** [`PromptCreator.Lookup`](../SimpleConsole/PromptCreator.cs)
returns `Option<Func<Result<string, string>>>` for the menu-choice → prompt-creator
dispatch. The only failure mode is "this number isn't in the table", and
there is nothing more useful to say about it than "absent". The dispatch
table doesn't know about menus or the wording of any user-facing message,
so it speaks the smallest honest type.

---

## `Result<TSuccess, TError>` — explicit failure with a reason

`Result<TSuccess, TError>` represents the outcome of a computation that
can either succeed or fail, where the failure carries a value of its own.
It has two cases:

- **`Success(TSuccess value)`** — the computation produced a value.
- **`Failure(TError error)`** — the computation failed; the error explains
  why.

The error type is a generic parameter, so a `Result` failure can be a
string, a custom error record, an enum, or an aggregated list of
validation errors — whatever the caller needs to make the failure
useful.

**Core operations:**

- `Map(T -> U) : Result<T, E> -> Result<U, E>` — transform the success
  value; pass the failure through.
- `Bind(T -> Result<U, E>) : Result<T, E> -> Result<U, E>` — chain
  another Result-returning step; the chain short-circuits on `Failure`.
  This is the basis of **railway-oriented programming**: success flows
  down the happy track, the first `Failure` shunts the rest of the
  computation onto the error track.
- `Match(T -> R, E -> R) : Result<T, E> -> R` — collapse to a single
  value by handling both cases.

**Example in this repo.** [`Program.ParseChoice`](../SimpleConsole/Program.cs)
returns `Result<int, string>` for parsing the user's menu input. There
are several distinguishable things that could go wrong (`""`, `"abc"`,
`"99999999999999999"`, …), and the caller — and ultimately the user
reading the error — needs to know which one happened. `"'abc' is not a
number."` is information; bare absence would not be.

---

## When to use which

The rule of thumb fits in one sentence:

> Use **`Option<T>`** when "absent" is the whole story.
> Use **`Result<T, E>`** when the failure carries information the caller
> will branch on or display.

A more careful test: imagine you're standing on the failure path. Is there
anything more useful you could say than "not there"? If yes — a reason, a
recovery hint, a code, a category — that's a `Result`. If not — and "we
don't have one" is the only meaningful message — that's an `Option`.

A second test: who consumes the failure? If a *programmer* writing the
next step in the pipeline only needs to know "value or no value", an
`Option` is enough. If a *user* will eventually see the failure, or if a
programmer needs to do different things for different failure reasons,
the failure needs a reason — `Result`.

### Reach for `Option<T>` when…

- Looking up an entity by ID in an in-memory cache or dictionary
- Finding the first element of a list matching a predicate
- Reading an optional configuration setting
- Asking "is this user currently logged in?"
- Walking up to the parent of a tree node (the root has none)
- Resolving a feature flag that may or may not be set
- Reading a nullable column from a query result
- Asking "what was the previously cached value for this key?"
- Looking up the next/previous item in a collection
- Resolving an environment variable that may not be defined

### Reach for `Result<TSuccess, TError>` when…

- Parsing user input (numbers, dates, e-mails, JSON)
- Validating a form field or a domain value
- Updating a database table
- Calling an external HTTP API
- Authenticating a user
- Loading and parsing a configuration file
- Compiling or interpreting source code
- Reading from a file
- Sending a message over a network or a queue
- Making a payment or any other transactional side effect
- Acquiring a lock or a lease
- Running a migration

### The boundary between them

The interesting moment is when an `Option` flows out of a lower layer and
needs to become a user-facing error. The lookup itself doesn't know the
*context* that makes the absence meaningful — that lives one layer up,
where the caller knows what was being looked up and why.
`Option<T>.ToResult(E)` is the boundary helper: at the call site, the
caller attaches the reason, and the rest of the pipeline runs on the
`Result` track.

`SimpleConsole`'s [`PromptCreator.CreatePrompt`](../SimpleConsole/PromptCreator.cs)
does exactly this: `Lookup` returns an `Option`, and the public method
calls `.ToResult("Invalid choice. Please select a number from the menu.")`
to attach the user-facing reason at the layer that knows what the menu
was. Two failure modes — parse failure and unknown-choice failure — flow
to the same `Match` at the top of `Program.cs` on the same rail, and they
each carry the message that the layer that knows about them chose to
attach.
