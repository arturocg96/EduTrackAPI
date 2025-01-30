using Microsoft.AspNetCore.Mvc;

namespace ApiCursos.Configuration.DependencyInjection;

public static class CacheConfiguration
{
    public static IServiceCollection AddCacheConfiguration(this IServiceCollection services)
    {
        services.AddResponseCaching();
        services.AddControllers(options =>
        {
            options.CacheProfiles.Add("Default30Seconds", new CacheProfile
            {
                Duration = 30
            });
        });

        return services;
    }
}
