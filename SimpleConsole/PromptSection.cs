namespace SimpleConsole;

// Shared rendering helper for prompt creators that follow the same shape:
// "a list of (title, PowerShell command) sections, each rendered as a
// labelled markdown block, all concatenated into one prompt body".
//
// Pulling this out turns each prompt creator into nothing more than its
// data — the sections array — plus a one-line call to Render. The shape
// of "what the prompt is" stays cleanly separated from "how it's rendered".
internal static class PromptSection
{
    // Build a full prompt body from a fixed list of sections.
    //
    // Functional notes:
    //   - Sections are *data*, not control flow. Each item is a pair of
    //     (title, command); the list is built once, declaratively.
    //   - We Select (map) each pair into its rendered string, then fold
    //     the strings together with string.Join. That's a fold over the
    //     "strings under concatenation" monoid (see the FP intro doc):
    //     associative combine, identity = "".
    //   - Per-section failures are absorbed inside RenderOne, not bubbled
    //     up. Diagnostic data wants best-effort: one bad query shouldn't
    //     lose the other nine.
    public static string Render(
        IEnumerable<(string Title, string Command)> sections,
        string preamble,
        string heading) =>
        preamble + "\n" + heading + "\n\n" +
        string.Join("\n", sections.Select(s => RenderOne(s.Title, s.Command)));

    // Run one section's command and turn its Result into a markdown block.
    // Match collapses the two tracks (success / failure) into a single
    // string, so the caller never has to branch on the Result itself.
    private static string RenderOne(string title, string command) =>
        Shell.RunPowerShell(command).Match(
            onSuccess: output => $"## {title}\n{FormatBody(output.Trim())}\n",
            onFailure: error  => $"## {title}\n_(could not collect: {error})_\n");

    private static string FormatBody(string text) =>
        string.IsNullOrEmpty(text)
            ? "_(no data)_"
            : "```\n" + text + "\n```";
}
