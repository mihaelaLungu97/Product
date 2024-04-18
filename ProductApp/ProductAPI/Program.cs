using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;
configuration.AddJsonFile("appsettings.json");

builder.Services.AddControllers();
builder.Services.AddDbContext<ProductContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddDbContext<ProductContext>(opt =>opt.UseInMemoryDatabase("ProductList"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
