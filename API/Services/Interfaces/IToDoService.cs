using API.Models;

namespace API.Services.Interfaces;

public interface IToDoService : IService<ToDoTask>
{
    public Task<ServiceResult<List<ToDoTask>>> GetAll(string? query);
    public Task<ServiceResult<List<ToDoTask>>> GetCompletedTasks(string? query);
    public Task<ServiceResult<List<ToDoTask>>> GetPendingTasks(string? query);
    public Task<ServiceResult<List<ToDoTask>>> GetOverdueTasks(string? query);
}