using DataExtractor.Core.Ports;

namespace DataExtractor.Core.Extractors;

public sealed class SimpleFieldExtractor(string column) : IFieldExtractor
{
    public string Header => column;

    public string Extract(IReadOnlyDictionary<string, string> record) => record.GetRequired(column);
}