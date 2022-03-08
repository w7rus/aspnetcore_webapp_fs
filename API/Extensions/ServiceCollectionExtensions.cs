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
        IHostEnvironment env,
        IConfiguration configuration
    )
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Information()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                Path.Combine(env.ContentRootPath, "Logs", $"log_error_{DateTime.UtcNow:yyyy_mm_dd}.log"),
                LogEventLevel.Error, rollingInterval: RollingInterval.Day, buffered: true,
                flushToDiskInterval: TimeSpan.FromMinutes(1), rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 4194304)
            .WriteTo.File(
                Path.Combine(env.ContentRootPath, "Logs", $"log_information_{DateTime.UtcNow:yyyy_mm_dd}.log"),
                LogEventLevel.Information, rollingInterval: RollingInterval.Day, buffered: true,
                flushToDiskInterval: TimeSpan.FromMinutes(1), rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 4194304);

        Log.Logger = loggerConfiguration.CreateLogger();

        serviceCollection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(Log.Logger, true));

        serviceCollection.AddSingleton(Log.Logger);

        return serviceCollection;
    }
}