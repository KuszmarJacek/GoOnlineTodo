using GoOnlineTodo.Entities.DbSet;
using Microsoft.EntityFrameworkCore;
using GoOnlineTodo.DataService.Data;
using GoOnlineTodo.Entities.DTOs;
using FluentValidation;
using GoOnlineTodo.Api.Extensions;
using GoOnlineTodo.Api.MinimalApis;
using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
}

builder.Services.AddValidators();
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySQL(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Go Online Assessment Todo REST API",
    });
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Separate static class for minimal apis so that program.cs doesn't become a mess
app.MapTodoApi();

app.Run();

