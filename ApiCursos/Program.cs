using ApiCursos.CoursesMapper;
using ApiCursos.Data;
using ApiCursos.Repository;
using ApiCursos.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Asp.Versioning;
using System.Numerics;
using Microsoft.AspNetCore.Identity;
using ApiCursos.Models;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);
var app = builder.Build();
ConfigureMiddleware(app);
app.Run();

static void ConfigureServices(WebApplicationBuilder builder)
{
    AddDatabase(builder);
    AddIdentity(builder);
    AddRepositories(builder);
    AddAuthentication(builder);
    AddVersioning(builder);
    AddSwagger(builder);
    ConfigureCors(builder);
    ConfigureCache(builder);
    AddAutoMapper(builder);
    builder.Services.AddControllers();
}

static void AddIdentity(WebApplicationBuilder builder)
{
    builder.Services
        .AddIdentity<UserApp, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
}

static void AddVersioning(WebApplicationBuilder builder)
{
    builder.Services.AddApiVersioning(opt =>
    {
        opt.DefaultApiVersion = new ApiVersion(1, 0);
        opt.AssumeDefaultVersionWhenUnspecified = false;
        opt.ReportApiVersions = true;
        opt.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new QueryStringApiVersionReader("api-version")
        );
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
        options.AssumeDefaultVersionWhenUnspecified = false;
    });
}

static void AddDatabase(WebApplicationBuilder builder)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));
}

static void AddRepositories(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<ICategory, CategoryRepository>();
    builder.Services.AddScoped<ICourse, CourseRepository>();
    builder.Services.AddScoped<IUser, UserRepository>();
}

static void AddAuthentication(WebApplicationBuilder builder)
{
    var key = builder.Configuration.GetValue<string>("ApiSettings:secretKey");
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        ConfigureJwtBearerOptions(options, key);
    });
}

static void ConfigureJwtBearerOptions(JwtBearerOptions options, string key)
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
}

static void AddSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));

        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "API Cursos V1",
            Version = "v1",
            Description = "Backend API Cursos V1",
            Contact = new OpenApiContact()
            {
                Name = "Arturo Carrasco González",
                Email = "arturocarrascogonzalez@hotmail.com",
                Url = new Uri("https://github.com/arturocg96/")
            },
            License = new OpenApiLicense()
            {
                Name = "Licencia Personal",
                Url = new Uri("https://tudominio.com/licencia")
            }
        });

        options.SwaggerDoc("v2", new OpenApiInfo
        {
            Title = "API Cursos V2",
            Version = "v2",
            Description = "Backend API Cursos V2",
            Contact = new OpenApiContact()
            {
                Name = "Arturo Carrasco González",
                Email = "arturocarrascogonzalez@hotmail.com",
                Url = new Uri("https://github.com/arturocg96/")
            },
            License = new OpenApiLicense()
            {
                Name = "Licencia Personal",
                Url = new Uri("https://tudominio.com/licencia")
            }
        });

        ConfigureSwaggerSecurity(options);
    });
}

static void ConfigureSwaggerSecurity(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticación JWT usando el esquema Bearer. \r\n\r\n " +
                      "Ingresa 'Bearer' [espacio] y después tu token en el campo de abajo \r\n\r\n" +
                      "Ejemplo: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
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

static void ConfigureCors(WebApplicationBuilder builder)
{
    builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
        policy => policy.WithOrigins("*")
                       .AllowAnyMethod()
                       .AllowAnyHeader()));
}

static void ConfigureCache(WebApplicationBuilder builder)
{
    builder.Services.AddResponseCaching();
    builder.Services.AddControllers(options =>
    {
        options.CacheProfiles.Add("Default30Seconds", new CacheProfile
        {
            Duration = 30
        });
    });
}

static void AddAutoMapper(WebApplicationBuilder builder)
{
    builder.Services.AddAutoMapper(typeof(CoursesMapper));
}

static void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Cursos V1");
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "API Cursos V2");
        });
    }
    app.UseStaticFiles();
    app.UseHttpsRedirection();
    app.UseCors("CorsPolicy");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
}