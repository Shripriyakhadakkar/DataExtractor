using DataExtractor.Core;
using DataExtractor.Core.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace DataExtractor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDataExtractor(this IServiceCollection services)
    {
        services.AddSingleton<IRecordReader, CsvHelperRecordReader>();
        services.AddSingleton<IRecordWriter, CsvHelperRecordWriter>();
        services.AddSingleton<BankProfileRegistry>();
        services.AddSingleton<ExtractionService>();

        // Every IBankProfile in this assembly is registered automatically: adding a bank needs no other change.
        var profileTypes = typeof(DependencyInjection).Assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IBankProfile).IsAssignableFrom(type));

        foreach (var profileType in profileTypes)
            services.AddSingleton(typeof(IBankProfile), profileType);

        return services;
    }
}