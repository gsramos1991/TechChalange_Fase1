using Asp.Versioning.ApiExplorer;
using FCG.Api.Configs;
using FCG.Infra.Data.Seed;
using Serilog;

namespace FCG.Api.Extensions;

public static class ApplicationExtensions
{
    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        var apiVersionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UseSwaggerConfig(apiVersionProvider, app.Environment);
        app.SeedDatabase();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCorrelationIdMiddleware();
        app.MapControllers();

        return app;
    }

    private static void SeedDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var seeder = services.GetRequiredService<DatabaseSeeder>();
            seeder.SeedAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao executar seed inicial da base de dados");
        }
    }
}