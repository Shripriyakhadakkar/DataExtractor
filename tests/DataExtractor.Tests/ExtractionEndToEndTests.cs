using DataExtractor.Core;
using DataExtractor.Core.Extractors;
using DataExtractor.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DataExtractor.Tests;

public class ExtractionEndToEndTests
{
    private const string BarclaysInput = """
        TimeZone=UTC,,,,
        Ignored,ISIN,CFICode,Venue,AlgoParams
        x,DE000C4SA5W8,FFICSX,XEUR,25.00000323187.50InstIdentCode:DE000C4SA5W8|;PriceMultiplier:25.0|;ExpiryDate:2020-09-18|;
        x,O:EVH\U20\12.5,OPASPS,XCBO,100.00000308.79PriceMultiplier:100.0|;OptionType:PUTO|;
        """;

    private static ExtractionService CreateService() =>
        new ServiceCollection().AddDataExtractor().BuildServiceProvider().GetRequiredService<ExtractionService>();

    private static string Run(ExtractionService service, string bank, string input)
    {
        var output = new StringWriter();
        service.Extract(bank, new StringReader(input), output);
        return output.ToString();
    }

    private static string[] Lines(string csv) =>
        csv.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    [Fact]
    public void Extract_WritesConfiguredFieldsAndContractSize_ForEachRow()
    {
        var lines = Lines(Run(CreateService(), "Barclays", BarclaysInput));

        Assert.Equal(
            new[]
            {
                "ISIN,CFICode,Venue,ContractSize",
                "DE000C4SA5W8,FFICSX,XEUR,25.0",
                @"O:EVH\U20\12.5,OPASPS,XCBO,100.0",
            },
            lines);
    }

    [Fact]
    public void Extract_IgnoresBankNameCase()
    {
        var lines = Lines(Run(CreateService(), "barclays", BarclaysInput));

        Assert.Equal(3, lines.Length);
    }

    [Fact]
    public void Extract_WritesRepeatedRowOnlyOnce()
    {
        var repeated = BarclaysInput + "\nx,DE000C4SA5W8,FFICSX,XEUR,25.00000323187.50PriceMultiplier:25.0|;";

        var lines = Lines(Run(CreateService(), "Barclays", repeated));

        Assert.Equal(3, lines.Length);
    }

    [Fact]
    public void Extract_ThrowsUnknownBankException_WhenBankHasNoProfile()
    {
        Assert.Throws<UnknownBankException>(() => Run(CreateService(), "Nobody", BarclaysInput));
    }

    [Fact]
    public void Extract_UsesPluggedInBankProfile_WithoutChangingCore()
    {
        var bankX = new FakeBankProfile("BankX", new SimpleFieldExtractor("Symbol"), new SimpleFieldExtractor("Qty"));
        var service = new ExtractionService(
            new BankProfileRegistry([bankX]), new CsvHelperRecordReader(), new CsvHelperRecordWriter());

        var lines = Lines(Run(service, "BankX", "Qty,Symbol,Notes\n5,VOD,hello\n"));

        Assert.Equal(new[] { "Symbol,Qty", "VOD,5" }, lines);
    }

    [Fact]
    public void Extract_ProducesTheExpectedFile_ForTheSampleBarclaysFile()
    {
        var input = File.ReadAllText(SamplePath("barclays_input.csv"));
        var expected = Lines(File.ReadAllText(SamplePath("barclays_expected_output.csv")));

        var actual = Lines(Run(CreateService(), "Barclays", input));

        Assert.Equal("ISIN,CFICode,Venue,ContractSize", actual[0]);
        Assert.Equal(expected, actual);
    }

    private static string SamplePath(string fileName) =>
        Path.Combine(AppContext.BaseDirectory, "samples", fileName);
}