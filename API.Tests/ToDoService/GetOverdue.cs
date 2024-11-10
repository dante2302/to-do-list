using API.Data;
using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Tests;

public class ToDoService__GetOverdue : IDisposable
{
    private readonly ToDoDbContext _dbContext;
    private readonly ToDoService _toDoService;

    public ToDoService__GetOverdue()
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

    #pragma warning disable

    [Fact]
    public async Task GetOverdueTasks_ShouldReturnOnlyOverdueTasks()
    {
        ToDoTask overdueTask = new()
        {
            Title = "Overdue Task",
            DueDate = DateTime.UtcNow.AddDays(-1),
            IsCompleted = false
        };

        ToDoTask futureTask = new()
        {
            Title = "Future Task",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        await _dbContext.AddAsync(futureTask);
        await _dbContext.AddAsync(overdueTask);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetOverdueTasks();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal("Overdue Task", result.Data[0].Title);
    }


    [Fact]
    public async Task GetOverdueTasks_ShouldReturnEmpty_WhenNoOverdueTasks()
    {
        ToDoTask futureTask = new()
        {
            Title = "Future Task",
            DueDate = DateTime.UtcNow.AddDays(5),
            IsCompleted = false
        };

        await _dbContext.AddAsync(futureTask);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetOverdueTasks();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetOverdueTasks_ShouldOnlyReturnOverdueNotCompletedTasks()
    {
        ToDoTask overdueTask = new()
        {
            Title = "Overdue Task",
            DueDate = DateTime.UtcNow.AddDays(-2),
            IsCompleted = false
        };

        ToDoTask completedTaskOverdue = new()
        {
            Title = "Completed Overdue Task",
            DueDate = DateTime.UtcNow.AddDays(-3),
            IsCompleted = true
        };

        await _dbContext.AddAsync(overdueTask);
        await _dbContext.AddAsync(completedTaskOverdue);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetOverdueTasks();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal("Overdue Task", result.Data[0].Title);
        Assert.False(result.Data[0].IsCompleted);
    }

    [Fact]
    public async Task GetOverdueTasks_ShouldNotReturnCompletedTasksWithFutureDueDates()
    {
        ToDoTask futureCompletedTask = new()
        {
            Title = "Future Completed Task",
            DueDate = DateTime.UtcNow.AddDays(5),
            IsCompleted = true
        };

        await _dbContext.AddAsync(futureCompletedTask);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetOverdueTasks();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetOverdueTasks_ShouldReturnOverdueOnlyTasks_WhenMultipleTasksExist()
    {
        ToDoTask overdueTask1 = new()
        {
            Title = "Overdue Task 1",
            DueDate = DateTime.UtcNow.AddDays(-2),
            IsCompleted = false
        };

        ToDoTask overdueTask2 = new()
        {
            Title = "Overdue Task 2",
            DueDate = DateTime.UtcNow.AddDays(-3),
            IsCompleted = false
        };

        ToDoTask futureTask = new()
        {
            Title = "Future Task",
            DueDate = DateTime.UtcNow.AddDays(2),
            IsCompleted = false
        };

        await _dbContext.AddAsync(overdueTask1);
        await _dbContext.AddAsync(overdueTask2);
        await _dbContext.AddAsync(futureTask);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetOverdueTasks();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);
        Assert.Contains(result.Data, t => t.Title == "Overdue Task 1");
        Assert.Contains(result.Data, t => t.Title == "Overdue Task 2");
    }

    [Fact]
    public async Task GetOverdueTasks_ShouldNotReturnCompletedFutureTasks()
    {
        ToDoTask futureCompletedTask = new()
        {
            Title = "Completed Future Task",
            DueDate = DateTime.UtcNow.AddDays(3),
            IsCompleted = true
        };

        await _dbContext.AddAsync(futureCompletedTask);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetOverdueTasks();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);  // Future tasks, even if completed, shouldn't show up in overdue tasks
    }

    [Fact]
    public async Task GetOverdueTasks_ShouldReturnTasksSortedByTitle()
    {
        await _dbContext.Tasks.AddRangeAsync(new List<ToDoTask>
        {
            new() { Title = "B Task", IsCompleted = false, DueDate = DateTime.UtcNow.AddDays(-2) },
            new() { Title = "A Task", IsCompleted = false, DueDate = DateTime.UtcNow.AddDays(-1) }
        });
        await _dbContext.SaveChangesAsync();

        var result = await _toDoService.GetOverdueTasks(null, "Title", "asc");

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("A Task", result.Data[0].Title);
    }

    [Fact]
    public async Task GetOverdueTasks_ShouldReturnEmptyIfNoOverdueTasks()
    {
        ServiceResult<List<ToDoTask>> result = await _toDoService.GetOverdueTasks();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);  // No overdue tasks should return an empty list
    }

    [Fact]
    public async Task GetOverdueTasks_ShouldHandleMultipleTasksWithVariousDueDates()
    {
        ToDoTask overdueTask1 = new()
        {
            Title = "Overdue Task 1",
            DueDate = DateTime.UtcNow.AddDays(-1),
            IsCompleted = false
        };

        ToDoTask overdueTask2 = new()
        {
            Title = "Overdue Task 2",
            DueDate = DateTime.UtcNow.AddDays(-2),
            IsCompleted = false
        };

        ToDoTask futureTask = new()
        {
            Title = "Future Task",
            DueDate = DateTime.UtcNow.AddDays(5),
            IsCompleted = false
        };

        await _dbContext.AddAsync(overdueTask1);
        await _dbContext.AddAsync(overdueTask2);
        await _dbContext.AddAsync(futureTask);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetOverdueTasks();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);  // Only overdue tasks should appear
    }
}