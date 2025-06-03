using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FCG.Api.Configs;

public static class JwtConfig
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        Serilog.ILogger logger,
        IWebHostEnvironment environment)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = jwtSettings["Key"];

        // Validação da chave JWT
        if (string.IsNullOrEmpty(key) || key.Length < 32)
        {
            string menssagem = "JWT Key não configurada ou inválida. A chave deve ter pelo menos 32 caracteres.";
            logger.Error(menssagem);
            throw new ArgumentException(menssagem);
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = !environment.IsDevelopment(); // Só permite HTTP em desenvolvimento

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = true,
                RequireSignedTokens = true,

                // Reduz o clock skew para maior segurança
                ClockSkew = TimeSpan.FromMinutes(5),

                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

                // Configurações adicionais de segurança
                ValidateActor = false,
                ValidateTokenReplay = false
            };

            // Eventos para logging e debugging
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (environment.IsDevelopment())
                    {
                        string menssagem = "JWT Authentication failed: " + (context.Exception?.Message ?? "Authentication failed");
                        logger.Information(menssagem);
                        Console.WriteLine(menssagem);
                    }
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    if (environment.IsDevelopment())
                    {
                        string menssagem = "JWT Token validated for user: " + context.Principal?.Identity?.Name;
                        logger.Information(menssagem);
                        Console.WriteLine(menssagem);
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    if (environment.IsDevelopment())
                    {
                        string menssagem = "JWT Challenge: " + context.ErrorDescription;
                        logger.Warning(menssagem);
                        Console.WriteLine(menssagem);
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

}

