using ApiCursos.Data;
using ApiCursos.Repository.IRepository;
using ApiCursos.Repository;
using Microsoft.EntityFrameworkCore;

namespace ApiCursos.Configuration.DependencyInjection;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ConexionSql")));

        services.AddScoped<ICategory, CategoryRepository>();
        services.AddScoped<ICourse, CourseRepository>();
        services.AddScoped<IUser, UserRepository>();

        services.AddAutoMapper(typeof(Program).Assembly);
        services.AddControllers();

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.UseCors("CorsPolicy");
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        return app;
    }
}
