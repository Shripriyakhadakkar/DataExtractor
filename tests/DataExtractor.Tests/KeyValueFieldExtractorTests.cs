using DataExtractor.Core.Extractors;

namespace DataExtractor.Tests;

public class KeyValueFieldExtractorTests
{
    private readonly KeyValueFieldExtractor _extractor = new("Contract Size", "AlgoParams", "PriceMultiplier");

    private string Extract(string algoParams) =>
        _extractor.Extract(new Dictionary<string, string> { ["AlgoParams"] = algoParams });

    [Fact]
    public void Extract_ReturnsValueOfRequestedKey()
    {
        Assert.Equal("25.0", Extract("InstFullName:DAX|;PriceMultiplier:25.0|;UnderlInstCode:DE0008469008|;"));
    }

    [Fact]
    public void Extract_IgnoresJunkBeforeFirstPair()
    {
        Assert.Equal("10.0", Extract("25.00000323187.50PriceMultiplier:10.0|;InstFullName:DAX|;"));
    }

    [Fact]
    public void Extract_ReturnsEmptyText_WhenKeyIsMissing()
    {
        Assert.Equal("", Extract("InstFullName:DAX|;"));
    }

    [Fact]
    public void Extract_DoesNotMatchLookAlikeKey()
    {
        Assert.Equal("", Extract("SubPriceMultiplier:5|;"));
    }

    [Fact]
    public void Header_ReturnsConfiguredHeader()
    {
        Assert.Equal("Contract Size", _extractor.Header);
    }
}