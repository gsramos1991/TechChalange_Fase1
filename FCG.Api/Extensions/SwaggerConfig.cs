using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace FCG.Api.Extensions;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // Configuração de autenticação JWT
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT no formato: Bearer {seu_token}"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Incluir comentários XML se existirem
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // Configurações adicionais
            c.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
        });

        return services;
    }


    public static IServiceCollection ConfigureSwaggerVersioning(
        this IServiceCollection services,
        IApiVersionDescriptionProvider provider)
    {
        services.Configure<SwaggerGenOptions>(options =>
        {
            // Limpar documentos existentes
            options.SwaggerGeneratorOptions.SwaggerDocs.Clear();

            // Criar um documento para cada versão da API
            foreach (var description in provider.ApiVersionDescriptions.OrderBy(v => v.ApiVersion))
            {
                options.SwaggerGeneratorOptions.SwaggerDocs.Add(
                    description.GroupName,
                    new OpenApiInfo
                    {
                        Title = "FCG API",
                        Version = description.ApiVersion.ToString(),
                        Description = description.IsDeprecated ?
                            "Esta versão da API foi descontinuada" :
                            "API para gerenciamento de produtos FCG\n\n👥 **Desenvolvido por:** \n\nCLOVIS ALCEU CASSARO (RM362482)\n\nGABRIEL SANTOS RAMOS (RM361692)\n\nJÚLIO CÉSAR DE CARVALHO (RM364945)\n\nMARCO ANTONIO ARAUJO (RM360981)\n\nYASMIM MUNIZ DA SILVA CARAÇA (RM362555)",
                        Contact = new OpenApiContact
                        {
                            //Name = "Equipe FCG",
                            //Email = "contato@fcg.com",
                            Url = new Uri("https://github.com/gsramos1991/TechChalange_Fase1")
                        }                    
                    });
            }
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfig(
        this IApplicationBuilder app,
        IApiVersionDescriptionProvider provider,
        IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // Criar uma página para cada versão
                foreach (var description in provider.ApiVersionDescriptions.OrderBy(v => v.ApiVersion))
                {
                    c.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        $"FCG API {description.GroupName.ToUpperInvariant()}");
                }

                c.RoutePrefix = "swagger";
                c.DocumentTitle = "FCG API Documentation";
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                c.DefaultModelsExpandDepth(-1);
                c.DisplayRequestDuration();
                c.EnableFilter();
                c.EnableTryItOutByDefault();
                c.EnablePersistAuthorization();

                // Configurações adicionais para melhorar a experiência
                c.ConfigObject.AdditionalItems.Add("persistAuthorization", true);
                c.ConfigObject.AdditionalItems.Add("displayOperationId", false);
                c.ConfigObject.AdditionalItems.Add("displayRequestDuration", true);

            });
        }

        return app;
    }
}