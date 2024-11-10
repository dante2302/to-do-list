using API.Models;

namespace API.Services.Interfaces;

public interface IToDoService : IService<ToDoTask>
{
    public Task<ServiceResult<List<ToDoTask>>> GetAll(string? query, string? orderBy, string? orderDir);
    public Task<ServiceResult<List<ToDoTask>>> GetCompletedTasks(string? query, string? orderBy, string? orderDir);
    public Task<ServiceResult<List<ToDoTask>>> GetPendingTasks(string? query, string? orderBy, string? orderDir);
    public Task<ServiceResult<List<ToDoTask>>> GetOverdueTasks(string? query, string? orderBy, string? orderDir);
}