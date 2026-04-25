using FunctionalDotNet;

namespace SimpleConsole;

internal static class PromptCreator
{
    // Pure dispatch from an integer choice to a Result<string, string>.
    //
    // Notes on the functional shape:
    //   - The body is a single switch *expression*, not a statement.
    //     It produces a value rather than mutating one — the move from
    //     imperative to functional in miniature.
    //   - The discard arm (`_ =>`) makes the function total: every int has
    //     a defined Result mapping, so the caller never needs an `else` and
    //     the function never throws for "unexpected" input.
    //   - Returning Result<,> on the unknown-choice path means the failure
    //     is part of the type signature; a caller cannot accidentally drop
    //     it the way an exception or a magic sentinel value can be dropped.
    public static Result<string, string> CreatePrompt(int choice) =>
        choice switch
        {
            1 => HardwareSpecPromptCreator.CreatePrompt(),
            2 => NetworkSpecPromptCreator.CreatePrompt(),
            _ => new Result<string, string>.Failure("Invalid choice. Please select a number from the menu.")
        };
}
