
namespace FunctionalCodingExercises;

public static class FizzBuzzExercise
{
    public static IEnumerable<string> FizzBuzz(int count) =>
        Enumerable.Range(1, count).Select(x =>
            x % 15 == 0 ? "FizzBuzz" :
            x % 3 == 0 ? "Fizz" :
            x % 5 == 0 ? "Buzz" :
            x.ToString());
}
