using DataExtractor.Core.Ports;

namespace DataExtractor.Infrastructure;

public sealed class CsvHelperRecordWriter : IRecordWriter
{
    public void Write(TextWriter output, IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows) =>
        throw new NotImplementedException();
}