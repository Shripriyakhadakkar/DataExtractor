using DataExtractor.Core.Ports;

namespace DataExtractor.Tests;

public sealed class FakeBankProfile(string bankName, params IFieldExtractor[] fields) : IBankProfile
{
    public string BankName => bankName;

    public IReadOnlyList<IFieldExtractor> Fields => fields;
}