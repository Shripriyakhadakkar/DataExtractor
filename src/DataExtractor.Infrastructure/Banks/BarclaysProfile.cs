using DataExtractor.Core.Extractors;
using DataExtractor.Core.Ports;

namespace DataExtractor.Infrastructure.Banks;

public sealed class BarclaysProfile : IBankProfile
{
    public string BankName => "Barclays";

    public IReadOnlyList<IFieldExtractor> Fields { get; } =
    [
        new SimpleFieldExtractor("ISIN"),
        new SimpleFieldExtractor("CFICode"),
        new SimpleFieldExtractor("Venue"),
        new KeyValueFieldExtractor(header: "Contract Size", sourceColumn: "AlgoParams", key: "PriceMultiplier"),
    ];

    public TextReader PreProcess(TextReader input)
    {
        input.ReadLine();
        return input;
    }
}