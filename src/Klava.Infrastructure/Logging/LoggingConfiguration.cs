namespace Klava.Infrastructure.Logging;

using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

public static class LoggingConfiguration
{
    public static IHostBuilder ConfigureKlavaLogging(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, services, configuration) =>
        {
            var logsPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            Directory.CreateDirectory(logsPath);

            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProperty("Application", "Klava")
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: Path.Combine(logsPath, "klava-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: Path.Combine(logsPath, "klava-errors-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 90,
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");
        });
    }
}
