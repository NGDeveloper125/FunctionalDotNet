using FunctionalCodingExercises;

namespace FunctionalCodingExercises.Tests;

public class PalindromeExerciseTests
{
    [Theory]
    [InlineData("civic")]
    [InlineData("racecar")]
    [InlineData("level")]
    [InlineData("a")]
    [InlineData("")]
    public void Recognises_palindromes(string input) =>
        Assert.True(PalindromeExercise.IsPalindrome(input));

    [Theory]
    [InlineData("hello")]
    [InlineData("functional")]
    [InlineData("ab")]
    public void Rejects_non_palindromes(string input) =>
        Assert.False(PalindromeExercise.IsPalindrome(input));

    [Theory]
    [InlineData("Civic")]
    [InlineData("RaceCar")]
    [InlineData("LEVEL")]
    public void Comparison_is_case_insensitive(string input) =>
        Assert.True(PalindromeExercise.IsPalindrome(input));
}
