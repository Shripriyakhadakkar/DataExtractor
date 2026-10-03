using DataExtractor.Core;

namespace DataExtractor.Tests;

public class BankProfileRegistryTests
{
    private readonly FakeBankProfile _alpha = new("Alpha");
    private readonly BankProfileRegistry _registry;

    public BankProfileRegistryTests()
    {
        _registry = new BankProfileRegistry([_alpha, new FakeBankProfile("Beta")]);
    }

    [Fact]
    public void Get_FindsBank_IgnoringCase()
    {
        Assert.Same(_alpha, _registry.Get("aLPHA"));
    }

    [Fact]
    public void Get_ThrowsUnknownBankException_WhenBankIsUnknown()
    {
        Assert.Throws<UnknownBankException>(() => _registry.Get("Gamma"));
    }

    [Fact]
    public void Get_ListsKnownBanksInMessage_WhenBankIsUnknown()
    {
        var error = Assert.Throws<UnknownBankException>(() => _registry.Get("Gamma"));

        Assert.Contains("Alpha", error.Message);
        Assert.Contains("Beta", error.Message);
    }
}