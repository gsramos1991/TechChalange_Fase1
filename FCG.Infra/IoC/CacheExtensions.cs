using FCG.Infra.Cache;
using Microsoft.Extensions.DependencyInjection;
using FCG.Core.Services.Interfaces;


namespace FCG.Infra.IoC;

public static class CacheExtensions
{
    public static IServiceCollection AddCacheServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemCacheService>();
        return services;
    }
}