namespace DataExtractor.Core.Ports
{

    /// <summary>Produces one output column from one input row.</summary>
    public interface IFieldExtractor
    {
        string Header { get; }
        string Extract(IReadOnlyDictionary<string, string> record);
    }
}