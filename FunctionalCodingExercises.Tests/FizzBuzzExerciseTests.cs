using FunctionalCodingExercises;

namespace FunctionalCodingExercises.Tests;

public class FizzBuzzExerciseTests
{
    [Fact]
    public void Count_of_zero_yields_an_empty_sequence() =>
        Assert.Empty(FizzBuzzExercise.FizzBuzz(0));

    [Fact]
    public void First_fifteen_values_follow_the_classic_pattern()
    {
        var expected = new[]
        {
            "1", "2", "Fizz", "4", "Buzz",
            "Fizz", "7", "8", "Fizz", "Buzz",
            "11", "Fizz", "13", "14", "FizzBuzz"
        };

        Assert.Equal(expected, FizzBuzzExercise.FizzBuzz(15));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(9)]
    public void Multiples_of_three_but_not_five_yield_Fizz(int n) =>
        Assert.Equal("Fizz", FizzBuzzExercise.FizzBuzz(n).Last());

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    public void Multiples_of_five_but_not_three_yield_Buzz(int n) =>
        Assert.Equal("Buzz", FizzBuzzExercise.FizzBuzz(n).Last());

    [Theory]
    [InlineData(15)]
    [InlineData(30)]
    [InlineData(45)]
    public void Multiples_of_fifteen_yield_FizzBuzz(int n) =>
        Assert.Equal("FizzBuzz", FizzBuzzExercise.FizzBuzz(n).Last());
}
