using DataExtractor.Core;
using DataExtractor.Core.Ports;
using DataExtractor.Infrastructure;
using DataExtractor.Infrastructure.Banks;
using Microsoft.Extensions.DependencyInjection;

namespace DataExtractor.Tests;

public class DependencyInjectionTests
{
    private static ServiceProvider Build() =>
        new ServiceCollection().AddDataExtractor().BuildServiceProvider();

    [Fact]
    public void AddDataExtractor_RegistersExtractionService()
    {
        using var provider = Build();

        Assert.NotNull(provider.GetRequiredService<ExtractionService>());
    }

    [Fact]
    public void AddDataExtractor_RegistersCsvHelperAdapters()
    {
        using var provider = Build();

        Assert.IsType<CsvHelperRecordReader>(provider.GetRequiredService<IRecordReader>());
        Assert.IsType<CsvHelperRecordWriter>(provider.GetRequiredService<IRecordWriter>());
    }

    [Fact]
    public void AddDataExtractor_RegistersEveryBankProfileInTheInfrastructureAssembly()
    {
        using var provider = Build();

        var profiles = provider.GetServices<IBankProfile>();

        Assert.Contains(profiles, profile => profile is BarclaysProfile);
    }

    [Fact]
    public void AddDataExtractor_MakesBarclaysAvailableInTheRegistry_IgnoringCase()
    {
        using var provider = Build();

        var profile = provider.GetRequiredService<BankProfileRegistry>().Get("barclays");

        Assert.Equal("Barclays", profile.BankName);
    }
}