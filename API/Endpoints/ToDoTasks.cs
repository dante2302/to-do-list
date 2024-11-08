using API.Models;
using API.Services;

namespace API.Endpoints;

public static class ToDoTasks
{
    private static readonly string _url = "/api/todos";
    public static void RegisterToDoTasksEndpoints(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder toDoTasks = routes.MapGroup(_url);

        toDoTasks.MapPost("", async (IToDoService service, ToDoTask task) => {
            ToDoTask? created = await service.Create(task);
            return created is null
                ? Results.BadRequest()
                : Results.Created($"{_url}/{ created.Id }", created);
        });

        toDoTasks.MapGet("", async (IToDoService service) => {
            List<ToDoTask> toDoTasks = await service.GetAll();
            return Results.Ok(toDoTasks);
        });
        
        toDoTasks.MapGet("/{id}", async (IToDoService service, int id) => 
        {
            ToDoTask? toDoTask = await service.GetOne(id);
            return toDoTask is null ? Results.NotFound() : Results.Ok(toDoTask);
        });

        toDoTasks.MapGet("/pending", async (IToDoService service) => {
            List<ToDoTask> pendingTasks = await service.GetPendingTasks();
            return Results.Ok(pendingTasks);
        });

        toDoTasks.MapGet("/completed", async (IToDoService service) => {
            List<ToDoTask> completedTasks = await service.GetCompletedTasks();
            return Results.Ok(completedTasks);
        });

        toDoTasks.MapGet("/overdue", async (IToDoService service) => {
            List<ToDoTask> overdueTasks = await service.GetOverdueTasks();
            return Results.Ok(overdueTasks);
        });

        toDoTasks.MapPut("/{id}", 
            async (IToDoService service, int id, ToDoTask updateTaskData) => {
                ToDoTask? taskAfterUpdate = await service.Update(updateTaskData);

                return taskAfterUpdate is null 
                    ? Results.NotFound() 
                    : Results.Ok(taskAfterUpdate);
        });

        toDoTasks.MapDelete("/{id}" , async (IToDoService service, int id) => {
            await service.Delete(id);
            return Results.NoContent();
        });
    }
}