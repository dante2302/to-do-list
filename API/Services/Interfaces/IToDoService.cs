using API.Models;

namespace API.Services.Interfaces;

public interface IToDoService : IService<ToDoTask>
{
    public Task<ServiceResult<List<ToDoTask>>> GetCompletedTasks();
    public Task<ServiceResult<List<ToDoTask>>> GetPendingTasks();
    public Task<ServiceResult<List<ToDoTask>>> GetOverdueTasks();
}