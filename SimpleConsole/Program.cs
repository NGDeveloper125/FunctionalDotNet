// AI Information Prompter
//
// Shape of this program: a thin I/O shell wrapped around a pure core.
//   1. read input from the console        (impure, edge)
//   2. run it through a Result pipeline   (pure, core)
//   3. write the rendered output          (impure, edge)
//
// The core is a chain of Result-returning steps composed with Bind. None of
// them touch the console or any other shared state, so they're trivially
// testable and trivially refactorable. The console calls are localised to
// the two boundaries — the read at the top and the write at the bottom —
// which is the standard FP layout: "functional core, imperative shell".

using FunctionalDotNet;
using SimpleConsole;

// I/O edge — necessarily impure. WriteLine has a side effect (writing to
// stdout) and ReadLine has both a side effect and a non-deterministic return
// value. We accept that here because we are at the boundary of the program.
Console.WriteLine("Choose a prompt type to create:");
Console.WriteLine("1. Hardware spec");
Console.WriteLine("2. Network spec");
string input = Console.ReadLine() ?? string.Empty;

// Pure core — a Result pipeline.
//
//   ParseChoice  : string -> Result<int, string>
//   CreatePrompt : int    -> Result<string, string>
//   Match        : Result<string, string> -> string
//
// Bind handles short-circuiting on failure for us: if ParseChoice returns a
// Failure, CreatePrompt is never called and the error flows straight into
// Match. The author writes only the happy path; the monad does the plumbing.
//
// Match is the pipeline's exit: it collapses the two tracks (success / error)
// into a single string we can hand to the I/O edge below.
var output = ParseChoice(input)
    .Bind(PromptCreator.CreatePrompt)
    .Match(
        onSuccess: prompt => $"Generated Prompt:\n{prompt}",
        onFailure: error  => $"Error:\n{error}");

// I/O edge again — the only place output is observed.
Console.WriteLine(output);

// Pure: same input string in, same Result out, no side effects.
//
// We lift int.TryParse's (bool, out int) shape into a Result so it composes
// with the rest of the pipeline via Bind, instead of forcing an imperative
// `if` and a discarded out-parameter at the call site.
static Result<int, string> ParseChoice(string input) =>
    int.TryParse(input, out var choice)
        ? new Result<int, string>.Success(choice)
        : new Result<int, string>.Failure($"'{input}' is not a number.");
