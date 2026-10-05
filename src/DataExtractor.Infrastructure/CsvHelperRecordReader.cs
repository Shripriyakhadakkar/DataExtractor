using System.Globalization;
using CsvHelper;
using DataExtractor.Core.Ports;

namespace DataExtractor.Infrastructure;

public sealed class CsvHelperRecordReader : IRecordReader
{
    public IEnumerable<IReadOnlyDictionary<string, string>> Read(TextReader input)
    {
        using var csv = new CsvReader(input, CultureInfo.InvariantCulture);

        var headers = AsInvalidData(() =>
        {
            csv.Read();
            csv.ReadHeader();
            return csv.HeaderRecord!;
        });

        while (AsInvalidData(csv.Read))
            yield return AsInvalidData(() => ReadRecord(csv, headers));
    }

    private static Dictionary<string, string> ReadRecord(CsvReader csv, string[] headers)
    {
        var record = new Dictionary<string, string>();
        for (var column = 0; column < headers.Length; column++)
            record[headers[column]] = csv.GetField(column) ?? string.Empty;

        return record;
    }

    private static T AsInvalidData<T>(Func<T> action)
    {
        try
        {
            return action();
        }
        catch (CsvHelperException ex)
        {
            throw new InvalidDataException($"The input is not valid CSV. {ex.Message}", ex);
        }
    }
}