using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiCursos.Configuration.DependencyInjection;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
            ConfigureSwaggerVersions(options);
            ConfigureSwaggerSecurity(options);
        });

        return services;
    }

    private static void ConfigureSwaggerVersions(SwaggerGenOptions options)
    {
        var contact = new OpenApiContact
        {
            Name = "Arturo Carrasco González",
            Email = "arturocarrascogonzalez@hotmail.com",
            Url = new Uri("https://github.com/arturocg96/")
        };

        var license = new OpenApiLicense
        {
            Name = "Licencia Personal",
            Url = new Uri("https://tudominio.com/licencia")
        };

        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "API Cursos V1",
            Version = "v1",
            Description = "Backend API Cursos V1",
            Contact = contact,
            License = license
        });

        options.SwaggerDoc("v2", new OpenApiInfo
        {
            Title = "API Cursos V2",
            Version = "v2",
            Description = "Backend API Cursos V2",
            Contact = contact,
            License = license
        });
    }

    private static void ConfigureSwaggerSecurity(SwaggerGenOptions options)
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Description = "Autenticación JWT usando el esquema Bearer. \r\n\r\n " +
                         "Ingresa 'Bearer' [espacio] y después tu token en el campo de abajo \r\n\r\n" +
                         "Ejemplo: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Scheme = "Bearer",
            Type = SecuritySchemeType.ApiKey
        };

        options.AddSecurityDefinition("Bearer", securityScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    }

    public static void UseSwaggerSetup(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Cursos V1");
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "API Cursos V2");
        });
    }
}