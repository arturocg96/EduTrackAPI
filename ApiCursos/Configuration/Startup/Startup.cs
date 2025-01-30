using ApiCursos.Configuration.DependencyInjection;

namespace ApiCursos.Configuration.Startup;

public class Startup
{
    private readonly WebApplicationBuilder _builder;
    private readonly IConfiguration _configuration;

    public Startup(WebApplicationBuilder builder)
    {
        _builder = builder;
        _configuration = builder.Configuration;
    }

    public void ConfigureServices()
    {
        _builder.Services
            .AddInfrastructure(_configuration)
            .AddIdentityConfiguration()
            .AddApiVersioningSetup()
            .AddSwaggerConfiguration()
            .AddCorsConfiguration()
            .AddCacheConfiguration()
            .AddApplicationServices();
    }

    public void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerSetup();
        }

        app.UseInfrastructure();
    }
}
