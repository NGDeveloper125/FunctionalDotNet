using FunctionalDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain;

public class NumberCompactor
{

    public static Result<string, string> CompressNumber(string? number)
    {
        if (string.IsNullOrEmpty(number))
        {
            return new Result<string, string>.Failure("Input number cannot be null or empty.");
        }
        if (!long.TryParse(number, out long numericValue))
        {
            return new Result<string, string>.Failure("Input must be a valid numeric string.");
        }

        return null;
    }
}
