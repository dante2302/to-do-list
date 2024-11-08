using API.Data;
using API.Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class ToDoService(ToDoDbContext dbContext) : IToDoService
{
    private readonly ToDoDbContext _dbContext = dbContext;

    public async Task<ServiceResult<ToDoTask>> Create(ToDoTask task)
    {
        if (string.IsNullOrEmpty(task.Title))
            return ServiceResult<ToDoTask>.Failure("Title is required.");

        if (task.DueDate < DateTime.Now)
            return ServiceResult<ToDoTask>.Failure("Due date cannot be in the past.");

        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync();

        return ServiceResult<ToDoTask>.Success(task);
    }

    public async Task<ServiceResult<ToDoTask>> GetOne(int id)
    {
        var task = await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
            return ServiceResult<ToDoTask>.Failure("Task not found.");

        return ServiceResult<ToDoTask>.Success(task);
    }

    public async Task<ServiceResult<List<ToDoTask>>> GetAll()
    {
        var tasks = await _dbContext.Tasks.ToListAsync();
        return ServiceResult<List<ToDoTask>>.Success(tasks);
    }

    public async Task<ServiceResult<List<ToDoTask>>> GetPendingTasks()
    {
        var tasks = await _dbContext.Tasks
            .Where(t => !t.IsCompleted && (t.DueDate == null || t.DueDate >= DateTime.UtcNow))
            .ToListAsync();

        return ServiceResult<List<ToDoTask>>.Success(tasks);
    }

    public async Task<ServiceResult<List<ToDoTask>>> GetCompletedTasks()
    {
        var tasks = await _dbContext.Tasks
            .Where(t => t.IsCompleted)
            .ToListAsync();

        return ServiceResult<List<ToDoTask>>.Success(tasks);
    }

    public async Task<ServiceResult<List<ToDoTask>>> GetOverdueTasks()
    {
        var tasks = await _dbContext.Tasks
            .Where(t => !t.IsCompleted && t.DueDate < DateTime.UtcNow)
            .ToListAsync();

        return ServiceResult<List<ToDoTask>>.Success(tasks);
    }

    public async Task<ServiceResult<ToDoTask>> Update(ToDoTask updatedTask)
    {
        var existingTask = await _dbContext.Tasks.FindAsync(updatedTask.Id);

        if (existingTask == null)
            return ServiceResult<ToDoTask>.Failure("Task not found.");

        existingTask.Title = updatedTask.Title;
        existingTask.DueDate = updatedTask.DueDate;
        existingTask.IsCompleted = updatedTask.IsCompleted;

        await _dbContext.SaveChangesAsync();
        return ServiceResult<ToDoTask>.Success(existingTask);
    }

    public async Task<ServiceResult<string>> Delete(int id)
    {
        var task = await _dbContext.Tasks.FindAsync(id);

        if (task == null)
            return ServiceResult<string>.Failure("Task not found.");

        _dbContext.Tasks.Remove(task);
        await _dbContext.SaveChangesAsync();

        return ServiceResult<string>.Success("Task deleted successfully.");
    }

}