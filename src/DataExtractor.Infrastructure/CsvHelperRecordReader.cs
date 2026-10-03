using DataExtractor.Core.Ports;

namespace DataExtractor.Infrastructure;

public sealed class CsvHelperRecordReader : IRecordReader
{
    public IEnumerable<IReadOnlyDictionary<string, string>> Read(TextReader input) =>
        throw new NotImplementedException();
}