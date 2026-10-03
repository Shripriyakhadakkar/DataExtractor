namespace DataExtractor.Core;

public sealed class UnknownBankException(string bankName, IEnumerable<string> knownBanks)
    : Exception($"No profile for bank '{bankName}'. Known banks: {string.Join(", ", knownBanks)}.");