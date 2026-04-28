using FunctionalDotNet;

namespace SimpleConsole;

internal static class PromptCreator
{
    // Pure dispatch from an integer choice to the prompt creator for that
    // choice — or absence, if the choice doesn't map to anything.
    //
    // Why Option<T> here, and not Result<T, E>?
    //   - The only failure mode is "this number isn't in the table". There
    //     is nothing more useful to attach to that failure than "absent":
    //     the dispatch table doesn't know about menus, users, or the
    //     phrasing of the message a UI might want to show.
    //   - The user-facing string "Invalid choice. Please select…" belongs
    //     at the layer that knows about the menu it printed (i.e. one
    //     level up). Burying it inside Lookup would couple the dispatch
    //     to one particular caller's error wording.
    //   - More generally: Option is the right type whenever "absent" is
    //     the whole story. Reach for Result the moment the failure has a
    //     reason the caller will branch on or display. See
    //     docs/discriminated-unions.md for the full discussion.
    private static Option<Func<Result<string, string>>> Lookup(int choice) =>
        choice switch
        {
            1 => new Option<Func<Result<string, string>>>.Some(HardwareSpecPromptCreator.CreatePrompt),
            2 => new Option<Func<Result<string, string>>>.Some(NetworkSpecPromptCreator.CreatePrompt),
            _ => new Option<Func<Result<string, string>>>.None()
        };

    // The boundary: Option -> Result.
    //
    //   ToResult lifts the Option onto the Result track by attaching the
    //   user-facing reason for absence. The "Invalid choice" string lives
    //   *here*, at the layer that knows about the menu, instead of inside
    //   the dispatch table.
    //
    //   Bind then runs the chosen creator (a thunk: Func<Result<...>>).
    //   The creator's own Result is folded back onto the same Failure
    //   track, so a failure deeper inside HardwareSpecPromptCreator flows
    //   through this expression on the same rail with no extra plumbing.
    public static Result<string, string> CreatePrompt(int choice) =>
        Lookup(choice)
            .ToResult("Invalid choice. Please select a number from the menu.")
            .Bind(creator => creator());
}
