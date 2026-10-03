using DataExtractor.Core;
using DataExtractor.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: DataExtraction <bank name> <input file>");
    return 1;
}

var (bankName, inputPath) = (args[0], args[1]);
var outputPath = Path.ChangeExtension(inputPath, null) + "_output.csv";

using var services = new ServiceCollection().AddDataExtractor().BuildServiceProvider();
var extraction = services.GetRequiredService<ExtractionService>();

try
{
    using var input = new StreamReader(inputPath);
    using var output = new StreamWriter(outputPath);

    extraction.Extract(bankName, input, output);

    Console.WriteLine($"Wrote {outputPath}");
    return 0;
}
catch (Exception ex) when (ex is UnknownBankException or InvalidDataException or IOException)
{
    File.Delete(outputPath); // do not leave a half-written file behind
    Console.Error.WriteLine(ex.Message);
    return 1;
}