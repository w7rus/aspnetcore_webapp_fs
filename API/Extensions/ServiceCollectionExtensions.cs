using BLL.Handlers;
using BLL.Services;
using Common.Options;

namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IFileService, FileService>();

        return serviceCollection;
    }

    public static IServiceCollection AddHandlers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IFileHandler, FileHandler>();

        return serviceCollection;
    }

    public static IServiceCollection AddCustomOptions(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        serviceCollection.AddOptions();
        serviceCollection.Configure<MiscOptions>(configuration.GetSection(nameof(MiscOptions)));
        serviceCollection.Configure<SeqOptions>(configuration.GetSection(nameof(SeqOptions)));

        return serviceCollection;
    }
}