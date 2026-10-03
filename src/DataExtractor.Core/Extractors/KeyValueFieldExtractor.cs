using System.Text.RegularExpressions;
using DataExtractor.Core.Ports;

namespace DataExtractor.Core.Extractors;

public sealed class KeyValueFieldExtractor(string header, string sourceColumn, string key) : IFieldExtractor
{
    private readonly Regex _pair = new($"(?<![A-Za-z]){Regex.Escape(key)}:(?<value>[^|]*)", RegexOptions.Compiled);

    public string Header => header;

    public string Extract(IReadOnlyDictionary<string, string> record)
    {
        var match = _pair.Match(record.GetRequired(sourceColumn));
        return match.Success ? match.Groups["value"].Value : string.Empty;
    }
}