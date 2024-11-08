using API.Models;

namespace API.Services;

public interface IToDoService : IService<ToDoTask>
{
    public Task<List<ToDoTask>> GetCompletedTasks();
    public Task<List<ToDoTask>> GetPendingTasks();
    public Task<List<ToDoTask>> GetOverdueTasks();
}