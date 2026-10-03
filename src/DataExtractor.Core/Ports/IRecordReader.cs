namespace DataExtractor.Core.Ports
{
    /// <summary>Reads CSV text into rows keyed by header name.</summary>
    public interface IRecordReader
    {
        IEnumerable<IReadOnlyDictionary<string, string>> Read(TextReader input);
    }
}