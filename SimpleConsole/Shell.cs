using System.Diagnostics;
using FunctionalDotNet;

namespace SimpleConsole;

internal static class Shell
{
    // Run a PowerShell -Command and capture its output.
    //
    // This function is *not* pure: it spawns a subprocess and reads from
    // the OS, so its return value depends on machine state, not just its
    // arguments. That's the point — it's the I/O edge that the prompt
    // creators sit on top of.
    //
    // What it *does* do, even at the edge, is keep the type discipline:
    //   - non-zero exit code  -> Failure (with stderr, or the exit code if
    //                             stderr was silent)
    //   - thrown exception    -> Failure (with the exception message)
    //   - zero exit code      -> Success (with trimmed stdout)
    //
    // So expected failures (PowerShell missing, command typo, query that
    // errors out) come back as values that callers can compose with Bind /
    // Match instead of bleeding through as exceptions.
    public static Result<string, string> RunPowerShell(string command)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
            // ArgumentList lets the runtime do per-argument quoting, so the
            // PowerShell command string is delivered intact regardless of
            // what quotes or special characters it contains.
            psi.ArgumentList.Add("-NoProfile");
            psi.ArgumentList.Add("-NonInteractive");
            psi.ArgumentList.Add("-Command");
            psi.ArgumentList.Add(command);

            using var process = Process.Start(psi);
            if (process is null)
                return new Result<string, string>.Failure("Failed to start PowerShell process.");

            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                var message = string.IsNullOrWhiteSpace(stderr)
                    ? $"PowerShell exited with code {process.ExitCode}."
                    : stderr.Trim();
                return new Result<string, string>.Failure(message);
            }

            return new Result<string, string>.Success(stdout.TrimEnd());
        }
        catch (Exception ex)
        {
            return new Result<string, string>.Failure(ex.Message);
        }
    }
}
