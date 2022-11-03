using System.Reflection;
using Microsoft.OpenApi.Models;

namespace Imagegram.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services)
    {
        // generate the XML docs that'll drive the swagger docs
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        
        services.AddSwaggerGen(ops =>
        {
            ops.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Imagegram.Api API",
                Version = "v1"
            });
            
            ops.IncludeXmlComments(xmlPath);
            ops.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = "Authorization",
                Scheme = "Bearer"
            });

            ops.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "Bearer",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        });
        
        return services;
    }

}