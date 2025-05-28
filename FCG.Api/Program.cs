// Configure Serilog
using Asp.Versioning.ApiExplorer;
using FCG.Api.Configuration;
using FCG.Api.Infra.Extensions;
using FCG.Api.Infra.Middleware;
using FCG.Business.Interfaces;
using FCG.Business.Models;
using FCG.Business.Services;
using FCG.Business.Services.Interfaces;
using FCG.Data;
using FCG.Data.Repository;
using FCG.Data.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;



try
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


    Log.Information("Iniciando WebApi");

    var builder = WebApplication.CreateBuilder(args);

    // Configurar Serilog
    builder.Host.UseSerilog(Log.Logger);

    // Controllers
    builder.Services.AddControllers();

    // Acesso so HttpContext
    builder.Services.AddHttpContextAccessor();

    // DbContext
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Identity
    builder.Services.AddIdentityConfig();

    // Autenticação JWT
    builder.Services.AddJwtAuthentication(builder.Configuration, Log.Logger, builder.Environment);

    // Repositórios
    builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

    // Serviços
    builder.Services.AddScoped<IProdutoService, ProdutoService>();

    // DatabaseSeeder
    builder.Services.AddScoped<DatabaseSeeder>();

    // Serviços customizados
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddCorrelationIdGenerator();


    builder.Services.AddApiVersioningConfig();


    builder.Services.AddEndpointsApiExplorer();

    // Swagger
    builder.Services.AddSwaggerConfig();
    builder.Services.AddApiVersioningConfig();
    builder.Services.AddEndpointsApiExplorer();

    var serviceProvider = builder.Services.BuildServiceProvider();
    var apiVersionDescriptionProvider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
    builder.Services.ConfigureSwaggerVersioning(apiVersionDescriptionProvider);


    var app = builder.Build();



    if (app.Environment.IsDevelopment())
    {
        var finalApiVersionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UseSwaggerConfig(finalApiVersionProvider, app.Environment);
    }

    // seed inicial do banco de dados:
    using (var scope = app.Services.CreateScope())
    {
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



    app.UseHttpsRedirection();


    app.UseSwaggerConfig(apiVersionDescriptionProvider, builder.Environment);

    // Adicione middleware de autenticação e autorização
    app.UseAuthentication();
    app.UseAuthorization();

    

    app.UseCorrelationIdMiddleware();

    app.MapControllers();


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

