using API;
using API.Data;
using API.Endpoints;
using API.Services;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>(options => {
    string connectionString = Configuration.Manager.GetConnectionString("DefaultConnection")
    ?? throw new InvalidDataException("Invalid Connection String!");

    options.UseNpgsql(connectionString, ef => ef.MigrationsAssembly("API"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // URL of the Vite frontend
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddScoped<IToDoService, ToDoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseCors("AllowFrontendApp");

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o => {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "To Do List");
        o.RoutePrefix = string.Empty;
    });
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
    db.Database.Migrate();
}

app.RegisterToDoTasksEndpoints();
app.Run();