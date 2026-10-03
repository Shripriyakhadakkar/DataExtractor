using DataExtractor.Core.Ports;

namespace DataExtractor.Core;

public sealed class ExtractionService(BankProfileRegistry banks, IRecordReader reader, IRecordWriter writer)
{
    public void Extract(string bankName, TextReader input, TextWriter output)
    {
        var profile = banks.Get(bankName);

        var headers = profile.Fields.Select(field => field.Header).ToList();
        var rows = reader.Read(profile.PreProcess(input))
            .Select(record => (IReadOnlyList<string>)profile.Fields.Select(field => field.Extract(record)).ToList())
            .DistinctBy(row => string.Join('\u001F', row));

        writer.Write(output, headers, rows);
    }
}