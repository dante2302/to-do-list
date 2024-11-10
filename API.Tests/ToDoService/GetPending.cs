using API.Data;
using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Tests;

public class ToDoService__GetPending : IDisposable
{
    private readonly ToDoDbContext _dbContext;
    private readonly ToDoService _toDoService;

    public ToDoService__GetPending()
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
    public async Task GetPendingTasks_ShouldReturnOnlyPendingTasks()
    {
        ToDoTask pendingTask = new()
        {
            Title = "Pending Task",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        ToDoTask completedTask = new()
        {
            Title = "Completed Task",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = true
        };

        await _dbContext.AddAsync(pendingTask);
        await _dbContext.AddAsync(completedTask);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetPendingTasks();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.False(result.Data[0].IsCompleted);
    }

    [Fact]
    public async Task GetPendingTasks_ShouldNotReturnCompletedTasksWithFutureDueDates()
    {
        ToDoTask futureCompletedTask = new()
        {
            Title = "Completed Future Task",
            DueDate = DateTime.UtcNow.AddDays(2),
            IsCompleted = true
        };

        await _dbContext.AddAsync(futureCompletedTask);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetPendingTasks();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetPendingTasks_ShouldReturnAllPendingTasksWithDifferentDueDates()
    {
        ToDoTask task1 = new()
        {
            Title = "Pending Task 1",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        ToDoTask task2 = new()
        {
            Title = "Pending Task 2",
            DueDate = DateTime.UtcNow.AddDays(5),
            IsCompleted = false
        };

        await _dbContext.AddAsync(task1);
        await _dbContext.AddAsync(task2);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetPendingTasks();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);
        Assert.Contains(result.Data, t => t.Title == "Pending Task 1" && !t.IsCompleted);
        Assert.Contains(result.Data, t => t.Title == "Pending Task 2" && !t.IsCompleted);
    }

    [Fact]
    public async Task GetPendingTasks_ShouldHandleTasksWithoutDueDate()
    {
        ToDoTask taskWithoutDueDate = new()
        {
            Title = "Task Without Due Date",
            DueDate = null,
            IsCompleted = false
        };

        await _dbContext.AddAsync(taskWithoutDueDate);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetPendingTasks();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Null(result.Data[0].DueDate);
    }

    [Fact]
    public async Task GetPendingTasks_ShouldReturnTasksWithSameTitle()
    {
        ToDoTask pendingTask1 = new()
        {
            Title = "Duplicate Task",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        ToDoTask pendingTask2 = new()
        {
            Title = "Duplicate Task",
            DueDate = DateTime.UtcNow.AddDays(5),
            IsCompleted = false
        };

        await _dbContext.AddAsync(pendingTask1);
        await _dbContext.AddAsync(pendingTask2);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetPendingTasks();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);  // Both tasks should appear in the result
    }

    [Fact]
    public async Task GetPendingTasks_ShouldHandleTasksWithFutureCompletionDates()
    {
        ToDoTask futureCompletedTask = new()
        {
            Title = "Completed Future Task",
            DueDate = DateTime.UtcNow.AddDays(5),
            IsCompleted = true
        };

        await _dbContext.AddAsync(futureCompletedTask);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetPendingTasks();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);  // Completed tasks should not show up in pending tasks
    }

    [Fact]
    public async Task GetPendingTasks_ShouldReturnMultipleTasksWithSameTitleButDifferentCompletionStatuses()
    {
        ToDoTask pendingTask = new()
        {
            Title = "Duplicate Task",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        ToDoTask completedTask = new()
        {
            Title = "Duplicate Task",
            DueDate = DateTime.UtcNow.AddDays(-1),
            IsCompleted = true
        };

        await _dbContext.AddAsync(pendingTask);
        await _dbContext.AddAsync(completedTask);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetPendingTasks();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal("Duplicate Task", result.Data[0].Title);  // Only the pending task should be returned
    }

    [Fact]
    public async Task GetPendingTasks_ShouldReturnPendingTasks_WithSearchAndOrder()
    {
        await _dbContext.Tasks.AddRangeAsync(new List<ToDoTask>
        {
            new() { Title = "Read Book", IsCompleted = false, DueDate = DateTime.UtcNow.AddDays(1) },
            new() { Title = "Write Code", IsCompleted = false, DueDate = DateTime.UtcNow.AddDays(2) },
            new() { Title = "Completed Task", IsCompleted = true, DueDate = DateTime.UtcNow.AddDays(2) }
        });
        await _dbContext.SaveChangesAsync();

        var result = await _toDoService.GetPendingTasks("Write", "Title", "asc");

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal("Write Code", result.Data[0].Title);
    }

    [Fact]
    public async Task GetPendingTasks_ShouldReturnPendingTasksOrderedByDueDate()
    {
        await _dbContext.Tasks.AddRangeAsync(new List<ToDoTask>
        {
            new() { Title = "Task 1", IsCompleted = false, DueDate = DateTime.UtcNow.AddDays(5) },
            new() { Title = "Task 2", IsCompleted = false, DueDate = DateTime.UtcNow.AddDays(1) }
        });
        await _dbContext.SaveChangesAsync();

        var result = await _toDoService.GetPendingTasks(null, "DueDate", "asc");

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("Task 2", result.Data[0].Title);
    }

    [Fact]
    public async Task GetPendingTasks_ShouldReturnTasksWithNullDueDate()
    {
        await _dbContext.Tasks.AddRangeAsync(new List<ToDoTask>
        {
            new() { Title = "Task with Null DueDate", IsCompleted = false, DueDate = null },
            new() { Title = "Future Task", IsCompleted = false, DueDate = DateTime.UtcNow.AddDays(2) }
        });
        await _dbContext.SaveChangesAsync();

        var result = await _toDoService.GetPendingTasks(null, null, null);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);
        Assert.Contains(result.Data, t => t.Title == "Task with Null DueDate");
    }
}