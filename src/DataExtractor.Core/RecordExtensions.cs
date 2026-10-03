namespace DataExtractor.Core;

public static class RecordExtensions
{
    /// <summary>Gets a column value, failing with a message that names the missing column.</summary>
    public static string GetRequired(this IReadOnlyDictionary<string, string> record, string column) =>
        record.TryGetValue(column, out var value)
            ? value
            : throw new InvalidDataException($"The input has no '{column}' column.");
}
