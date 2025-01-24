using ApiCursos.CoursesMapper;
using ApiCursos.Data;
using ApiCursos.Repository;
using ApiCursos.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Usa nombres únicos para esquemas basados en el nombre completo del tipo (namespace + nombre)
    options.CustomSchemaIds(type => type.FullName);
});





builder.Services.AddAutoMapper(typeof(CoursesMapper));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
