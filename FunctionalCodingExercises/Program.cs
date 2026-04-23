using FunctionalCodingExercises;

var exercises = new (string Name, Action Run)[]
{
    ("FizzBuzz",      RunFizzBuzz),
    ("Prime Numbers", RunPrimeNumbers),
    ("Palindrome",    RunPalindrome),
};

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Functional Programming Exercises ===");
    for (int i = 0; i < exercises.Length; i++)
        Console.WriteLine($"  {i + 1}. {exercises[i].Name}");
    Console.WriteLine("  0. Exit");
    Console.Write("\nSelect an exercise: ");

    var choice = Console.ReadLine();
    if (choice == "0") break;
    if (!int.TryParse(choice, out var index) || index < 1 || index > exercises.Length)
    {
        Console.WriteLine("Invalid selection.");
        continue;
    }

    Console.WriteLine();
    exercises[index - 1].Run();
}

static void RunFizzBuzz()
{
    var count = PromptNonNegativeInt("How many numbers? ");
    foreach (var line in FizzBuzzExercise.FizzBuzz(count))
        Console.WriteLine(line);
}

static void RunPrimeNumbers()
{
    var range = PromptNonNegativeInt("Upper bound (exclusive): ");
    foreach (var n in PrimeNumbersExercise.GetPrimeNumbers(range))
        Console.WriteLine(n);
}

static void RunPalindrome()
{
    Console.Write("Enter a string: ");
    var input = Console.ReadLine() ?? string.Empty;
    Console.WriteLine(PalindromeExercise.IsPalindrome(input) ? "Palindrome" : "Not a palindrome");
}

static int PromptNonNegativeInt(string message)
{
    while (true)
    {
        Console.Write(message);
        if (int.TryParse(Console.ReadLine(), out var value) && value >= 0)
            return value;
        Console.WriteLine("Please enter a non-negative integer.");
    }
}
