using BLL.Handlers;
using BLL.Services;
using Common.Options;
using Serilog;
using Serilog.Events;

namespace ASP.NET_Core_Web_Application_File_Server.Extensions;

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

        return serviceCollection;
    }

    public static IServiceCollection AddCustomLogging(
        this IServiceCollection serviceCollection,
        Serilog.ILogger logger,
        IWebHostEnvironment env
    )
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Logger(_ =>
            {
                _.MinimumLevel.Error()
                    .WriteTo.File(
                        Path.Combine(Directory.GetCurrentDirectory(), "Logs",
                            $"log_error_{DateTime.UtcNow:yyyy_mm_dd}.log"),
                        LogEventLevel.Error, rollingInterval: RollingInterval.Day);
            });


        if (env.IsDevelopment())
        {
            loggerConfiguration
                .WriteTo.Logger(_ =>
                {
                    _.MinimumLevel.Information()
                        .WriteTo.Console()
                        .WriteTo.File(
                            Path.Combine(Directory.GetCurrentDirectory(), "Logs",
                                $"log_debug_{DateTime.UtcNow:yyyy_mm_dd}.log"),
                            rollingInterval: RollingInterval.Day);
                });
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        serviceCollection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

        serviceCollection.AddSingleton(Log.Logger);

        return serviceCollection;
    }
}