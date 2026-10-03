using DataExtractor.Core.Ports;

namespace DataExtractor.Infrastructure.Banks;

public sealed class BarclaysProfile : IBankProfile
{
    public string BankName => throw new NotImplementedException();

    public IReadOnlyList<IFieldExtractor> Fields => throw new NotImplementedException();
}