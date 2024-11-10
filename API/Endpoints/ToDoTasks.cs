using API.Models;
using API.Services;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class ToDoTasks
{
    private static readonly string _url = "/api/todos";
    public static void RegisterToDoTasksEndpoints(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder toDoTasks = routes.MapGroup(_url);

        toDoTasks.MapPost("", async (IToDoService service, ToDoTask task) => {
            ServiceResult<ToDoTask> result = await service.Create(task);
            return result.IsSuccess
                ? Results.Created($"{_url}/{result.Data?.Id}", result.Data)
                : Results.BadRequest(result.Message);
        });

        toDoTasks.MapGet("/all", async (
            IToDoService service, 
            [FromQuery(Name = "title")]string? query
        ) => 
        {
            ServiceResult<List<ToDoTask>> result = await service.GetAll(query);
            return result.IsSuccess
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Message);
        });

        toDoTasks.MapGet("/{id}", async (IToDoService service, int id) => 
        {
            ServiceResult<ToDoTask> result = await service.GetOne(id);
            return result.IsSuccess
                ? Results.Ok(result.Data)
                : Results.NotFound(result.Message);
        });

        toDoTasks.MapGet("/pending", async (
            IToDoService service, 
            [FromQuery(Name = "title")]string? query
        ) => 
        {
            ServiceResult<List<ToDoTask>> result = await service.GetPendingTasks(query);

            return result.IsSuccess
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Message);
        });

        toDoTasks.MapGet("/completed", async (
            IToDoService service, 
            [FromQuery(Name = "title")]string? query
        ) => 
        {
            ServiceResult<List<ToDoTask>> result = await service.GetCompletedTasks(query);
            return result.IsSuccess
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Message);
        });

        toDoTasks.MapGet("/overdue", async (
            IToDoService service, 
            [FromQuery(Name = "title")]string? query
        ) => 
        {
            ServiceResult<List<ToDoTask>> result = await service.GetOverdueTasks(query);
            return result.IsSuccess
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Message);
        });

        toDoTasks.MapPut("/{id}", async (IToDoService service, int id, ToDoTask updateTaskData) => {
            ServiceResult<ToDoTask> result = await service.Update(updateTaskData);
            return result.IsSuccess
                ? Results.Ok(result.Data)
                : Results.NotFound(result.Message);
        });

        toDoTasks.MapDelete("/{id}", async (IToDoService service, int id) => {
            ServiceResult<string> result = await service.Delete(id);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Message);
        });
    }
}
