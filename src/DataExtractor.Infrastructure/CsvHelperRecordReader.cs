using System.Globalization;
using CsvHelper;
using DataExtractor.Core.Ports;

namespace DataExtractor.Infrastructure;

public sealed class CsvHelperRecordReader : IRecordReader
{
    public IEnumerable<IReadOnlyDictionary<string, string>> Read(TextReader input)
    {
        using var csv = new CsvReader(input, CultureInfo.InvariantCulture);

        csv.Read();
        csv.ReadHeader();
        var headers = csv.HeaderRecord!;

        while (csv.Read())
        {
            var record = new Dictionary<string, string>();
            for (var column = 0; column < headers.Length; column++)
                record[headers[column]] = csv.GetField(column) ?? string.Empty;

            yield return record;
        }
    }
}