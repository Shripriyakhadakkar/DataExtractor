namespace DataExtractor.Core.Ports;

/// <summary>Writes the extracted rows out as CSV.</summary>
public interface IRecordWriter
{
    void Write(TextWriter output, IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows);
}