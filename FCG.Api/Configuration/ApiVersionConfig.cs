using Asp.Versioning;

namespace FCG.Api.Configuration;

public static class ApiVersionConfig
{
    public static IServiceCollection AddApiVersioningConfig(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            // Define a versão padrão da API
            options.DefaultApiVersion = new ApiVersion(1, 0);

            // Se nenhuma versão for especificada, usa a padrão
            options.AssumeDefaultVersionWhenUnspecified = true;

            // Formas de especificar a versão (removido MediaTypeApiVersionReader para simplificar)
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),           // exemplo: api/v1/produtos
                new QueryStringApiVersionReader("version"), // api/produtos?version=1.0
                new HeaderApiVersionReader("X-Version")     // Header: X-Version: 1.0
            );

            // Informa as versões disponíveis nos headers de resposta
            options.ReportApiVersions = true;

            // Adiciona comportamento para versões não suportadas
            options.UnsupportedApiVersionStatusCode = 400;
        })
        .AddMvc()
        .AddApiExplorer(setup =>
        {
            // Formato da versão no grupo (v'major'.'minor')
            setup.GroupNameFormat = "'v'VVV";

            // Substitui automaticamente a versão nos controllers
            setup.SubstituteApiVersionInUrl = true;

            // Adiciona informações sobre parâmetros de versão obsoletos
            setup.AssumeDefaultVersionWhenUnspecified = true;
        });



        return services;
    }
}