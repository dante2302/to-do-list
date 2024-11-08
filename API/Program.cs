using API;
using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>(options => {
    string connectionString = Configuration.Manager.GetConnectionString("DefaultConnection")
    ?? throw new InvalidDataException("No connection string");

    options.UseNpgsql(connectionString, ef => ef.MigrationsAssembly("API"));
});

var app = builder.Build();
app.Run();