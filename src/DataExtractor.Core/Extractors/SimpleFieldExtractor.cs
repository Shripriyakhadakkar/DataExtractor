using DataExtractor.Core.Ports;

namespace DataExtractor.Core.Extractors;

public sealed class SimpleFieldExtractor(string column) : IFieldExtractor
{
    public string Header => throw new NotImplementedException();

    public string Extract(IReadOnlyDictionary<string, string> record) => throw new NotImplementedException();
}