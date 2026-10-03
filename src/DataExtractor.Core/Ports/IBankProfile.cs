namespace DataExtractor.Core.Ports;

/// <summary>
/// Everything that is specific to one bank. To support a new bank, add one class that implements this.
/// </summary>

public interface IBankProfile
{
    string BankName { get; }

    /// <summary>The output columns, in order.</summary>
    IReadOnlyList<IFieldExtractor> Fields { get; }

    /// <summary>Repairs files that are not valid CSV. Most banks need nothing, so the default is a pass-through.</summary>
    TextReader PreProcess(TextReader input) => input;
}