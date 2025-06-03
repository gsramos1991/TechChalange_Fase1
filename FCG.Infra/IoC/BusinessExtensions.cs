using FCG.Business.Services.Interfaces;
using FCG.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Infra.IoC;

/// <summary>
/// Extensões para configuração de serviços de negócio
/// </summary>
public static class BusinessExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        // Serviços de negócio
        services.AddScoped<IJogoService, JogoService>();

        return services;
    }
}