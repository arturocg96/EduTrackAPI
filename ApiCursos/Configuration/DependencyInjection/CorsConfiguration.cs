namespace ApiCursos.Configuration.DependencyInjection;

public static class CorsConfiguration
{
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy("CorsPolicy",
            policy => policy.WithOrigins("*")
                           .AllowAnyMethod()
                           .AllowAnyHeader()));

        return services;
    }
}
