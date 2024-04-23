using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Test.Data.DbContexts;
using Test.Data.Repositories;
using Test.Service.Helpers;
using Test.Service.Interfaces;
using Test.Service.Meppers;
using Test.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
.AddControllers()
.AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddAutoMapper(typeof(MapperProfile));

//Set Database Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionString")));

var app = builder.Build();
WebHostEnviromentHelper.WebRootPath = Path.GetFullPath("wwwroot");

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

