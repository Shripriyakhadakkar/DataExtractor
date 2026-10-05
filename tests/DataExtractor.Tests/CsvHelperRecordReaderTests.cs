using DataExtractor.Infrastructure;

namespace DataExtractor.Tests;

public class CsvHelperRecordReaderTests
{
    private static List<IReadOnlyDictionary<string, string>> Read(string csv) =>
        new CsvHelperRecordReader().Read(new StringReader(csv)).ToList();

    [Fact]
    public void Read_ReturnsOneRecordPerRow_KeyedByHeaderName()
    {
        var records = Read("ISIN,Venue\nA1,X1\nA2,X2\n");

        Assert.Equal(2, records.Count);
        Assert.Equal("A1", records[0]["ISIN"]);
        Assert.Equal("X2", records[1]["Venue"]);
    }

    [Fact]
    public void Read_ReturnsNoRecords_WhenInputHasOnlyHeaders()
    {
        Assert.Empty(Read("ISIN,Venue\n"));
    }

    [Fact]
    public void Read_KeepsBackslashesAndColonsUnchanged()
    {
        var records = Read("ISIN,Venue\nO:EVH\\U20\\12.5,XCBO\n");

        Assert.Equal(@"O:EVH\U20\12.5", records[0]["ISIN"]);
    }

    [Fact]
    public void Read_KeepsCommaInsideQuotedValue()
    {
        var records = Read("ISIN,Venue\n\"A,1\",X1\n");

        Assert.Equal("A,1", records[0]["ISIN"]);
    }

    [Fact]
    public void Read_ThrowsInvalidDataException_WhenARowHasFewerFieldsThanTheHeader()
    {
        var reader = new CsvHelperRecordReader();

        Assert.Throws<InvalidDataException>(
            () => reader.Read(new StringReader("ISIN,Venue\nA1\n")).ToList());
    }
}