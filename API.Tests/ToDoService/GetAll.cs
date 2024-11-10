
using API.Data;
using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Tests;

#pragma warning disable

public class ToDoService__GetAll : IDisposable
{
    private readonly ToDoDbContext _dbContext;
    private readonly ToDoService _toDoService;

    public ToDoService__GetAll()
    {
        var options = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase(databaseName: "ToDoDbTest_Specific")
            .Options;

        _dbContext = new ToDoDbContext(options);
        _toDoService = new ToDoService(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllTasks()
    {
        ToDoTask task1 = new()
        {
            Title = "Task 1",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        ToDoTask task2 = new()
        {
            Title = "Task 2",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        await _dbContext.AddAsync(task1);
        await _dbContext.AddAsync(task2);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetAll();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data?.Count);
    }

    [Fact]
    public async Task GetAll_ShouldReturnTasksOrderedByTitleAscending()
    {
        await _dbContext.Tasks.AddRangeAsync(new List<ToDoTask>
        {
            new() { Title = "Zebra Task", IsCompleted = false, DueDate = DateTime.UtcNow.AddDays(1) },
            new() { Title = "Apple Task", IsCompleted = true, DueDate = DateTime.UtcNow.AddDays(-1) }
        });
        await _dbContext.SaveChangesAsync();

        var result = await _toDoService.GetAll(null, "Title", "asc");

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("Apple Task", result.Data[0].Title);
    }

    [Fact]
    public async Task GetAll_ShouldReturnTasksOrderedByDueDateDescending()
    {
        await _dbContext.Tasks.AddRangeAsync(new List<ToDoTask>
        {
            new() { Title = "Task 1", IsCompleted = false, DueDate = DateTime.UtcNow.AddDays(5) },
            new() { Title = "Task 2", IsCompleted = true, DueDate = DateTime.UtcNow.AddDays(1) }
        });
        await _dbContext.SaveChangesAsync();

        var result = await _toDoService.GetAll(null, "DueDate", "desc");

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("Task 1", result.Data[0].Title);
    }

    [Fact]
    public async Task GetAll_ShouldHandleNonExistingOrderByProperty()
    {
        await _dbContext.Tasks.AddAsync(new ToDoTask { Title = "Task", IsCompleted = false, DueDate = new DateTime() });
        await _dbContext.SaveChangesAsync();

        var result = await _toDoService.GetAll(null, "NonExistentProperty", "asc");

        Assert.False(result.IsSuccess);
        Assert.Equal("Property NonExistentProperty doesn't exist!", result.Message);
    }
}