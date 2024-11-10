using API.Data;
using API.Helpers;
using API.Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

#pragma warning disable
public class ToDoService(ToDoDbContext dbContext) : IToDoService
{
    private readonly ToDoDbContext _dbContext = dbContext;

    public async Task<ServiceResult<ToDoTask>> Create(ToDoTask task)
    {
        if (string.IsNullOrEmpty(task.Title))
            return ServiceResult<ToDoTask>.Failure("Title is required.");

        if (task.DueDate < DateTime.UtcNow)
            return ServiceResult<ToDoTask>.Failure($"Due date cannot be in the past.");

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

    public async Task<ServiceResult<List<ToDoTask>>> GetAll(
        string? search=null, 
        string? orderBy=null, 
        string? orderDir=null)
    {
        var tasksQuery = _dbContext.Tasks.AsQueryable();
        ServiceResult<IQueryable<ToDoTask>> res = SearchAndSortTasks(tasksQuery, search, orderBy, orderDir);
        if(!res.IsSuccess)
            return ServiceResult<List<ToDoTask>>.Failure(res.Message);
        var taskList = await res.Data.ToListAsync();
        return ServiceResult<List<ToDoTask>>.Success(taskList);
    }

    public async Task<ServiceResult<List<ToDoTask>>> GetPendingTasks(
        string? search=null, 
        string? orderBy=null, 
        string? orderDir=null
    )
    {
        var tasksQuery =  _dbContext.Tasks
            .Where(t => !t.IsCompleted && (t.DueDate == null || t.DueDate >= DateTime.UtcNow));
        ServiceResult<IQueryable<ToDoTask>> res = SearchAndSortTasks(tasksQuery, search, orderBy, orderDir);
        if(!res.IsSuccess)
            return ServiceResult<List<ToDoTask>>.Failure(res.Message);
        var taskList = await res.Data.ToListAsync();
        return ServiceResult<List<ToDoTask>>.Success(taskList);
    }

    public async Task<ServiceResult<List<ToDoTask>>> GetCompletedTasks(
        string? search=null, 
        string? orderBy=null, 
        string? orderDir=null
    )
    {
        var tasksQuery =  _dbContext.Tasks
            .Where(t => t.IsCompleted);
        ServiceResult<IQueryable<ToDoTask>> res = SearchAndSortTasks(tasksQuery, search, orderBy, orderDir);

        if(!res.IsSuccess)
            return ServiceResult<List<ToDoTask>>.Failure(res.Message);

        var taskList = await res.Data.ToListAsync();
        return ServiceResult<List<ToDoTask>>.Success(taskList);
    }

    public async Task<ServiceResult<List<ToDoTask>>> GetOverdueTasks(
        string? search=null, 
        string? orderBy=null, 
        string? orderDir=null
    )
    {
        var tasksQuery =  _dbContext.Tasks
            .Where(t => !t.IsCompleted && t.DueDate < DateTime.UtcNow);

        ServiceResult<IQueryable<ToDoTask>> res = SearchAndSortTasks(tasksQuery, search, orderBy, orderDir);

        if(!res.IsSuccess)
            return ServiceResult<List<ToDoTask>>.Failure(res.Message);

        var taskList = await res.Data.ToListAsync();
        return ServiceResult<List<ToDoTask>>.Success(taskList);
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

    private static ServiceResult<IQueryable<ToDoTask>> SearchAndSortTasks(
        IQueryable<ToDoTask> tasksQuery, 
        string? search, 
        string? orderBy, 
        string? orderDir
    )
    {
        if(!string.IsNullOrEmpty(search))
            tasksQuery = tasksQuery.Where(t => t.Title.Contains(search));

        if(!string.IsNullOrEmpty(orderBy))
        {
            try
            {
                orderDir = orderDir?.ToLower();
                bool descending =  orderDir == "descending" || orderDir == "desc";
                tasksQuery = tasksQuery.OrderByDynamic(orderBy, descending);
            }
            catch(ArgumentException)
            {
                return ServiceResult<IQueryable<ToDoTask>>
                    .Failure($"Property {orderBy} doesn't exist!");
            }
        }
        return ServiceResult<IQueryable<ToDoTask>>.Success(tasksQuery);
    }
}