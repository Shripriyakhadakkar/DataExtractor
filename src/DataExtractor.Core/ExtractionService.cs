using DataExtractor.Core.Ports;

namespace DataExtractor.Core;

public sealed class ExtractionService(BankProfileRegistry banks, IRecordReader reader, IRecordWriter writer)
{
    public void Extract(string bankName, TextReader input, TextWriter output)
    {
        _ = (banks, reader, writer); // only silences the unused-parameter warning during the red step
        throw new NotImplementedException();
    }
}