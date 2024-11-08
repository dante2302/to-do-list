using System.Linq.Expressions;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class ToDoTaskRepository(ToDoDbContext context) : IRepository<ToDoTask>
{
    private readonly DbContext _context = context;

    public async Task Create(ToDoTask toDoTask)
    {
        await _context.AddAsync(toDoTask);
        await _context.SaveChangesAsync();
    }

    public async Task<ToDoTask?> GetOne(int id)
    {
       ToDoTask? task =  await _context.Set<ToDoTask>().FirstOrDefaultAsync(e => e.Id == id);
       return task;
    }

    public async Task<List<ToDoTask>> GetAll()
    {
        List<ToDoTask> tasks = await _context.Set<ToDoTask>().ToListAsync();
        return tasks;
    }

    public async Task<List<ToDoTask>> GetAllByFilter(Expression<Func<ToDoTask, bool>> expression)
    {
        List<ToDoTask> filteredTasks = await _context
            .Set<ToDoTask>()
            .Where(expression)
            .ToListAsync();

        return filteredTasks;
    }

    public async Task Update(ToDoTask taskBeforeUpdate, ToDoTask taskAfterUpdate)
    {
        _context
            .Entry(taskBeforeUpdate)
            .CurrentValues
            .SetValues(taskAfterUpdate);

        await _context.SaveChangesAsync();
    }

    public async Task Delete(ToDoTask task)
    {
        _context.Remove(task);
        await _context.SaveChangesAsync();
    }
}