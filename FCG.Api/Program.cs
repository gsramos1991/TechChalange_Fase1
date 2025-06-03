using FCG.Api.Extensions;
using FCG.Api.Middleware;
using FCG.Infra.IoC;
using Serilog;

try
{
    // Configure Serilog
    LoggingExtensions.ConfigureSerilog();
    Log.Information("Iniciando WebApi");

    var builder = WebApplication.CreateBuilder(args);
    builder.AddLogging();

    // Configuração dos serviços de forma organizada e híbrida
    builder.Services
        .AddDataServices(builder.Configuration)                 // FCG.Infra - Data Access
        .AddBusinessServices()                                              // FCG.Infra - Business Services
        .AddCorrelationServices()                                           // FCG.Infra - Correlation ID
        .AddCacheServices()                                                 // FCG.Infra - Memory Cache 
        .AddWebServices(builder.Configuration)                  // FCG.Api - Web específico
        .AddAuthenticationServices(builder.Configuration,       // FCG.Api - Auth/Identity
                                        builder.Environment, 
                                        Log.Logger);

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    var app = builder.Build();

    app.UseExceptionHandler();

    app.ConfigureApplication();

    Log.Information("Finalizando bootstrap da WebApi");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}