using Domain;

namespace FunctionalDotNet.Tests;

public class NumberCompactorTests
{
    [Fact]
    public void CompressNumber_ShouldReturnAnError_WhenNumberIsNull()
    {
        Result<string, string> result = NumberCompactor.CompressNumber(null);
        Assert.IsType<Result<string, string>.Failure>(result);
    }

    [Fact]
    public void CompressNumber_ShouldReturnAnError_WhenNumberIsEmpty()
    {
        Result<string, string> result = NumberCompactor.CompressNumber(string.Empty);
        Assert.IsType<Result<string, string>.Failure>(result);
    }

    [Fact]
    public void CompressNumber_ShouldReturnAnError_WhenNumberIsNotNumeric()
    {
        Result<string, string> result = NumberCompactor.CompressNumber("abc");
        Assert.IsType<Result<string, string>.Failure>(result);
    }
}
