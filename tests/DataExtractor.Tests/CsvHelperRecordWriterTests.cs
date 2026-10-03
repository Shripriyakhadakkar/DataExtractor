using DataExtractor.Infrastructure;

namespace DataExtractor.Tests;

public class CsvHelperRecordWriterTests
{
    private static string Write(string[] headers, params string[][] rows)
    {
        var output = new StringWriter();
        new CsvHelperRecordWriter().Write(output, headers, rows);
        return output.ToString();
    }

    private static string[] Lines(string csv) =>
        csv.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    [Fact]
    public void Write_WritesHeadersThenRows()
    {
        var csv = Write(["ISIN", "Venue"], ["A1", "X1"], ["A2", "X2"]);

        Assert.Equal(new[] { "ISIN,Venue", "A1,X1", "A2,X2" }, Lines(csv));
    }

    [Fact]
    public void Write_QuotesValue_WhenItContainsComma()
    {
        var csv = Write(["ISIN"], ["A,1"]);

        Assert.Equal("\"A,1\"", Lines(csv)[1]);
    }

    [Fact]
    public void Write_LeavesBackslashesUnchanged()
    {
        var csv = Write(["ISIN"], [@"O:EVH\U20\12.5"]);

        Assert.Equal(@"O:EVH\U20\12.5", Lines(csv)[1]);
    }

    [Fact]
    public void Write_DoesNotCloseTheOutput()
    {
        var output = new StringWriter();

        new CsvHelperRecordWriter().Write(output, ["ISIN"], []);

        output.Write("still open");
        Assert.EndsWith("still open", output.ToString());
    }
}