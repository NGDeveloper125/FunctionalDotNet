
using System.Text;

namespace FunctionalCodingExercises;

public static class PalindromeExercise
{
    public static bool IsPalindrome(string input) =>
        string.Equals(input, Reverse(input), StringComparison.OrdinalIgnoreCase);

    private static string Reverse(string input)
    {
        var sb = new StringBuilder();
        for (int i = input.Length - 1; i >= 0; i--)
        {
            sb.Append(input[i]);
        }
        return sb.ToString();
    }
}
