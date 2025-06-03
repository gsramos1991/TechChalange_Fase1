using FCG.Api.Configs;
using FCG.Infra.Token;

namespace FCG.Api.Extensions;

/// <summary>
/// Extensões para configuração de autenticação e autorização
/// </summary>
public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        Serilog.ILogger logger)
    {
        // Identity, JWT, Token Service
        services.AddIdentityConfig();
        services.AddJwtAuthentication(configuration, logger, environment);
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}