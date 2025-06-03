using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Builder;

namespace FCG.Api.Extensions;

/// <summary>
/// Extensões para configuração de logging
/// </summary>
public static class LoggingExtensions
{
    public static void ConfigureSerilog()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .WriteTo.File("logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] CorrelationId:{CorrelationId} {Message}{NewLine}{Exception}")
            .CreateLogger();
    }

    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(Log.Logger);
        return builder;
    }
}