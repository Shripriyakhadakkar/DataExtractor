using DataExtractor.Core.Ports;

namespace DataExtractor.Core;

public sealed class BankProfileRegistry(IEnumerable<IBankProfile> profiles)
{
    private readonly Dictionary<string, IBankProfile> _byName =
        profiles.ToDictionary(profile => profile.BankName, StringComparer.OrdinalIgnoreCase);

    public IBankProfile Get(string bankName) =>
        _byName.TryGetValue(bankName, out var profile)
            ? profile
            : throw new UnknownBankException(bankName, _byName.Keys);
}