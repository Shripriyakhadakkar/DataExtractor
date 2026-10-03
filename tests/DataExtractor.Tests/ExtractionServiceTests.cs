using DataExtractor.Core;
using DataExtractor.Core.Extractors;
using DataExtractor.Core.Ports;

namespace DataExtractor.Tests;

public class ExtractionServiceTests
{
    private readonly FakeRecordWriter _writer = new();

    private ExtractionService CreateService(FakeRecordReader reader, params IBankProfile[] profiles) =>
        new(new BankProfileRegistry(profiles), reader, _writer);

    private static FakeBankProfile AlphaProfile() =>
        new("Alpha", new SimpleFieldExtractor("ISIN"), new SimpleFieldExtractor("Venue"));

    private static void Run(ExtractionService service, string bank) =>
        service.Extract(bank, new StringReader(""), new StringWriter());

    [Fact]
    public void Extract_WritesHeadersOfProfileFieldsInOrder()
    {
        var service = CreateService(new FakeRecordReader(), AlphaProfile());

        Run(service, "Alpha");

        Assert.Equal(new[] { "ISIN", "Venue" }, _writer.Headers);
    }

    [Fact]
    public void Extract_WritesOneRowPerRecord_WithOnlyTheProfileFields()
    {
        var reader = new FakeRecordReader(
            new Dictionary<string, string> { ["ISIN"] = "A1", ["Venue"] = "X1", ["Ignored"] = "zzz" },
            new Dictionary<string, string> { ["ISIN"] = "A2", ["Venue"] = "X2", ["Ignored"] = "yyy" });
        var service = CreateService(reader, AlphaProfile());

        Run(service, "Alpha");

        Assert.Equal(2, _writer.Rows.Count);
        Assert.Equal(new[] { "A1", "X1" }, _writer.Rows[0]);
        Assert.Equal(new[] { "A2", "X2" }, _writer.Rows[1]);
    }

    [Fact]
    public void Extract_WritesRepeatedRowOnlyOnce()
    {
        var repeated = new Dictionary<string, string> { ["ISIN"] = "A1", ["Venue"] = "X1" };
        var service = CreateService(new FakeRecordReader(repeated, repeated), AlphaProfile());

        Run(service, "Alpha");

        Assert.Single(_writer.Rows);
    }

    [Fact]
    public void Extract_ThrowsUnknownBankException_WhenBankHasNoProfile()
    {
        var service = CreateService(new FakeRecordReader(), AlphaProfile());

        Assert.Throws<UnknownBankException>(() => Run(service, "Nobody"));
    }

    [Fact]
    public void Extract_ReadsFromPreProcessedInput()
    {
        var cleaned = new StringReader("cleaned");
        var reader = new FakeRecordReader();
        var service = CreateService(reader, new CleaningProfile(cleaned));

        service.Extract("Cleaner", new StringReader("raw"), new StringWriter());

        Assert.Same(cleaned, reader.ReceivedInput);
    }

    private sealed class CleaningProfile(TextReader cleaned) : IBankProfile
    {
        public string BankName => "Cleaner";

        public IReadOnlyList<IFieldExtractor> Fields => [];

        public TextReader PreProcess(TextReader input) => cleaned;
    }
}