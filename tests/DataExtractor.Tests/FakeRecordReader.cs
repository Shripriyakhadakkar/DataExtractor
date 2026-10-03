using DataExtractor.Core.Ports;

namespace DataExtractor.Tests;

public sealed class FakeRecordReader(params Dictionary<string, string>[] records) : IRecordReader
{
    public TextReader? ReceivedInput { get; private set; }

    public IEnumerable<IReadOnlyDictionary<string, string>> Read(TextReader input)
    {
        ReceivedInput = input;
        return records;
    }
}