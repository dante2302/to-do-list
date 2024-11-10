using API.Models;

namespace API.Services.Interfaces;

public interface IToDoService : IService<ToDoTask>
{
    public Task<ServiceResult<List<ToDoTask>>> GetAll(string? query=null, string? orderBy=null, string? orderDir=null);
    public Task<ServiceResult<List<ToDoTask>>> GetCompletedTasks(string? query=null, string? orderBy=null, string? orderDir=null);
    public Task<ServiceResult<List<ToDoTask>>> GetPendingTasks(string? query=null, string? orderBy=null, string? orderDir=null);
    public Task<ServiceResult<List<ToDoTask>>> GetOverdueTasks(string? query=null, string? orderBy=null, string? orderDir=null);
}