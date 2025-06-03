using FCG.Core.Services.Interfaces;
using FCG.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Infra.IoC;

/// <summary>
/// Extensões para configuração do Correlation ID
/// </summary>
public static class CorrelationExtensions
{
    public static IServiceCollection AddCorrelationServices(this IServiceCollection services)
    {
        services.AddTransient<ICorrelationIdGenerator, CorrelationIdGenerator>();

        return services;
    }
}