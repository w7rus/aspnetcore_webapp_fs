using API.AuthHandlers;
using API.Configuration;
using BLL.Handlers;
using BLL.Services;
using Common.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

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
    
    public static IServiceCollection AddCustomConfigureOptions(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IConfigureOptions<AuthenticationOptions>, ConfigureAuthenticationOptions>();
        serviceCollection
            .AddSingleton<IConfigureOptions<AccessTokenAuthenticationSchemeOptions>, ConfigureAccessTokenOptions>();

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