using DataExtractor.Infrastructure.Banks;
using DataExtractor.Core.Ports;

namespace DataExtractor.Tests;

public class BarclaysProfileTests
{
    private readonly IBankProfile _profile = new BarclaysProfile();

    [Fact]
    public void BankName_IsBarclays()
    {
        Assert.Equal("Barclays", _profile.BankName);
    }

    [Fact]
    public void Fields_AreIsinCfiCodeVenueAndContractSize_InThatOrder()
    {
        var headers = _profile.Fields.Select(field => field.Header);

        Assert.Equal(new[] { "ISIN", "CFICode", "Venue", "Contract Size" }, headers);
    }

    [Fact]
    public void Fields_ExtractContractSizeFromPriceMultiplierInAlgoParams()
    {
        var record = new Dictionary<string, string>
        {
            ["ISIN"] = "DE000C4SA5W8",
            ["CFICode"] = "FFICSX",
            ["Venue"] = "XEUR",
            ["AlgoParams"] = "25.00000323187.50InstIdentCode:DE000C4SA5W8|;PriceMultiplier:25.0|;",
        };

        var row = _profile.Fields.Select(field => field.Extract(record));

        Assert.Equal(new[] { "DE000C4SA5W8", "FFICSX", "XEUR", "25.0" }, row);
    }

    [Fact]
    public void PreProcess_SkipsTheTimeZoneLineSoTheHeaderComesFirst()
    {
        var cleaned = _profile.PreProcess(new StringReader("TimeZone=UTC,,,\nISIN,Venue\nA1,X1\n"));

        Assert.Equal("ISIN,Venue", cleaned.ReadLine());
    }

    [Fact]
    public void PreProcess_KeepsEveryLineAfterTheFirst()
    {
        var cleaned = _profile.PreProcess(new StringReader("TimeZone=UTC,,,\nISIN,Venue\nA1,X1"));

        Assert.Equal("ISIN,Venue\nA1,X1", cleaned.ReadToEnd());
    }
}