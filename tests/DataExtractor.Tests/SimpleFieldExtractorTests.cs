using DataExtractor.Core.Extractors;

namespace DataExtractor.Tests;

public class SimpleFieldExtractorTests
{
    private readonly SimpleFieldExtractor _extractor = new("ISIN");

    [Fact]
    public void Extract_ReturnsColumnValueUnchanged()
    {
        var record = new Dictionary<string, string> { ["ISIN"] = @"O:EVH\U20\12.5" };

        Assert.Equal(@"O:EVH\U20\12.5", _extractor.Extract(record));
    }

    [Fact]
    public void Extract_ThrowsInvalidDataException_WhenColumnIsMissing()
    {
        var error = Assert.Throws<InvalidDataException>(() => _extractor.Extract(new Dictionary<string, string>()));

        Assert.Contains("ISIN", error.Message);
    }
}