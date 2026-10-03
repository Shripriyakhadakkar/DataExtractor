using DataExtractor.Core.Ports;

namespace DataExtractor.Tests;

public sealed class FakeRecordWriter : IRecordWriter
{
    public IReadOnlyList<string> Headers { get; private set; } = [];

    public List<IReadOnlyList<string>> Rows { get; } = [];

    public void Write(TextWriter output, IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows)
    {
        Headers = headers;
        Rows.AddRange(rows);
    }
}