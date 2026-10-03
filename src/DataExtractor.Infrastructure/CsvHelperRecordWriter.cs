using System.Globalization;
using CsvHelper;
using DataExtractor.Core.Ports;

namespace DataExtractor.Infrastructure;

public sealed class CsvHelperRecordWriter : IRecordWriter
{
    public void Write(TextWriter output, IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows)
    {
        using var csv = new CsvWriter(output, CultureInfo.InvariantCulture, leaveOpen: true);

        WriteRow(csv, headers);
        foreach (var row in rows)
            WriteRow(csv, row);
    }

    private static void WriteRow(CsvWriter csv, IEnumerable<string> values)
    {
        foreach (var value in values)
            csv.WriteField(value);
        csv.NextRecord();
    }
}