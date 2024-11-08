using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services;
public class ToDoService(ToDoDbContext dbContext) : IToDoService
{
    private readonly ToDoDbContext _dbContext = dbContext;

    public async Task<ToDoTask?> Create(ToDoTask task)
    {
        if(string.IsNullOrEmpty(task.Title))
            return null;

        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync();
        return task;
    }

    public async Task<ToDoTask?> GetOne(int id)
    {
        return await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<ToDoTask>> GetAll()
    {
        return await _dbContext.Tasks.ToListAsync();
    }

    public async Task<List<ToDoTask>> GetPendingTasks()
    {
        return await _dbContext.Tasks
            .Where(t => !t.IsCompleted && (t.DueDate == null || t.DueDate >= DateTime.UtcNow))
            .ToListAsync();
    }

    public async Task<List<ToDoTask>> GetCompletedTasks()
    {
        return await _dbContext.Tasks
            .Where(t => t.IsCompleted)
            .ToListAsync();
    }

    public async Task<List<ToDoTask>> GetOverdueTasks()
    {
        return await _dbContext.Tasks
            .Where(t => !t.IsCompleted && t.DueDate < DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<ToDoTask?> Update(ToDoTask updatedTask)
    {
        var existingTask = await _dbContext.Tasks.FindAsync(updatedTask.Id);
        if (existingTask == null)
            return null;

        existingTask.Title = updatedTask.Title;
        existingTask.DueDate = updatedTask.DueDate;
        existingTask.IsCompleted = updatedTask.IsCompleted;

        await _dbContext.SaveChangesAsync();
        return existingTask;
    }

    public async Task Delete(int id)
    {
        var task = await _dbContext.Tasks.FindAsync(id);
        if (task != null)
        {
            _dbContext.Tasks.Remove(task);
            await _dbContext.SaveChangesAsync();
        }
    }
}