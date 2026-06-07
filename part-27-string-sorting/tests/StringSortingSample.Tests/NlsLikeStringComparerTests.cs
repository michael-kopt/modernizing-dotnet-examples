using StringSortingSample.Sorting;
using Xunit;

namespace StringSortingSample.Tests;

public class NlsLikeStringComparerTests
{
    [Fact]
    public void SortsWithLowercaseBeforeUppercaseWhenPrimaryKeyMatches()
    {
        var values = new List<string> { "TEST", "Test", "test" };

        values.Sort(NlsLikeStringComparer.Instance);

        Assert.Equal(new[] { "test", "Test", "TEST" }, values);
    }

    [Fact]
    public void SortsIgnoringHyphensInPrimaryComparison()
    {
        var values = new List<string> { "test-2", "test", "test-1" };

        values.Sort(NlsLikeStringComparer.Instance);

        Assert.Equal(new[] { "test", "test-1", "test-2" }, values);
    }

    [Fact]
    public void SortsBaseLettersBeforeDiacritics()
    {
        var values = new List<string> { "café", "Cafe", "cafe" };

        values.Sort(NlsLikeStringComparer.Instance);

        Assert.Equal(new[] { "cafe", "café", "Cafe" }, values);
    }

    [Fact]
    public void ProducesDeterministicOrderForMixedSample()
    {
        var values = new List<string> { "test-1", "test", "Test-2", "TEST" };

        values.Sort(NlsLikeStringComparer.Instance);

        Assert.Equal(new[] { "test", "test-1", "Test-2", "TEST" }, values);
    }
}
