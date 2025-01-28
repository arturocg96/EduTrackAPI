using ApiCursos.CoursesMapper;
using ApiCursos.Data;
using ApiCursos.Repository;
using ApiCursos.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

//using Microsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));

//builder.Services.AddControllers().AddNewtonsoftJson(options =>
//    {
//        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
//    }
//    );

//Repositories
builder.Services.AddScoped<ICategory, CategoryRepository>();
builder.Services.AddScoped<ICourse, CourseRepository>();
builder.Services.AddScoped<IUser, UserRepository>();

var key = builder.Configuration.GetValue<string>("ApiSettings:secretKey");

builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Usa nombres únicos para esquemas basados en el nombre completo del tipo (namespace + nombre)
    options.CustomSchemaIds(type => type.FullName);

});
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "Autenticación JWT usando el esquema Bearer. \r\n\r\n " +
        "Ingresa la palabra 'Bearer' segiodp de un [espacio] y desùés su token en el campo de abajo. \r\n\r\n " +
        "Ejemplo: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFydHVyb2NnIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzM4MDUzMzQ3LCJleHAiOjE3Mzg2NTgxNDcsImlhdCI6MTczODA1MzM0N30.oAl90mUD2rdNl2KzBtG_mILg2eX1U1acGw7vtV40zC4\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
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
            Scheme = "ouath2",
            Name = "Bearer",
            In = ParameterLocation.Header
        },
        new List<string>()
        }
    });
}
);

builder.Services.AddCors(p => p.AddPolicy("CorsPolicy", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));



builder.Services.AddAutoMapper(typeof(CoursesMapper));

//Authentication configuration
builder.Services.AddAuthentication(
    x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    }
);

 
    


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
