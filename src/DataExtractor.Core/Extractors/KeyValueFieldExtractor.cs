using DataExtractor.Core.Ports;

namespace DataExtractor.Core.Extractors;

public sealed class KeyValueFieldExtractor(string header, string sourceColumn, string key) : IFieldExtractor
{
    public string Header => throw new NotImplementedException();

    public string Extract(IReadOnlyDictionary<string, string> record) => throw new NotImplementedException();
}