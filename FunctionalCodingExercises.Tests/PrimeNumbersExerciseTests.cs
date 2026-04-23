using FunctionalCodingExercises;

namespace FunctionalCodingExercises.Tests;

public class PrimeNumbersExerciseTests
{
    [Fact]
    public void Range_of_zero_yields_an_empty_sequence() =>
        Assert.Empty(PrimeNumbersExercise.GetPrimeNumbers(0));

    [Fact]
    public void Range_of_two_yields_no_primes_because_upper_bound_is_exclusive() =>
        Assert.Empty(PrimeNumbersExercise.GetPrimeNumbers(2));

    [Fact]
    public void Primes_below_ten() =>
        Assert.Equal(new[] { 2, 3, 5, 7 }, PrimeNumbersExercise.GetPrimeNumbers(10));

    [Fact]
    public void Primes_below_twenty() =>
        Assert.Equal(new[] { 2, 3, 5, 7, 11, 13, 17, 19 }, PrimeNumbersExercise.GetPrimeNumbers(20));

    [Fact]
    public void Primes_below_fifty() =>
        Assert.Equal(
            new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 },
            PrimeNumbersExercise.GetPrimeNumbers(50));
}
