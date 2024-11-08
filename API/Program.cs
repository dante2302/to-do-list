using API;
using API.Data;
using API.Endpoints;
using API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>(options => {
    string connectionString = Configuration.Manager.GetConnectionString("DefaultConnection")
    ?? throw new InvalidDataException("No connection string");

    options.UseNpgsql(connectionString, ef => ef.MigrationsAssembly("API"));
});

builder.Services.AddScoped<IToDoService, ToDoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.RegisterToDoTasksEndpoints();
app.Run();