using API.Data;
using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Tests;

public class ToDoService__GetCompleted : IDisposable
{
    private readonly ToDoDbContext _dbContext;
    private readonly ToDoService _toDoService;

    public ToDoService__GetCompleted()
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
    public async Task GetCompletedTasks_ShouldReturnOnlyCompletedTasks()
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

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetCompletedTasks();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.True(result.Data[0].IsCompleted);
    }

    [Fact]
    public async Task GetCompletedTasks_ShouldReturnCompletedTaskWithNullDueDate()
    {
        ToDoTask completedTaskWithNullDueDate = new()
        {
            Title = "Completed Task with Null DueDate",
            DueDate = null,
            IsCompleted = true
        };

        await _dbContext.AddAsync(completedTaskWithNullDueDate);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetCompletedTasks();

        Assert.True(result.IsSuccess);
        Assert.True(result.Data[0].IsCompleted);
        Assert.Null(result.Data[0].DueDate);
    }

    [Fact]
    public async Task GetCompletedTasks_ShouldReturnCompletedTasksWithPastDueDates()
    {
        ToDoTask completedTaskPastDue = new()
        {
            Title = "Completed Task Past Due",
            DueDate = DateTime.UtcNow.AddDays(-3),
            IsCompleted = true
        };

        await _dbContext.AddAsync(completedTaskPastDue);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetCompletedTasks();
        Console.WriteLine(result.Data);
        Assert.True(result.IsSuccess);
        Assert.True(result.Data[0].IsCompleted);
        Assert.Equal("Completed Task Past Due", result.Data[0].Title);
    }

    [Fact]
    public async Task GetCompletedTasks_ShouldReturnCompletedTasksWithFutureDueDates()
    {
        ToDoTask completedTaskFutureDue = new()
        {
            Title = "Completed Task Future Due",
            DueDate = DateTime.UtcNow.AddDays(2),
            IsCompleted = true
        };

        await _dbContext.AddAsync(completedTaskFutureDue);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetCompletedTasks();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.True(result.Data[0].IsCompleted);
        Assert.Equal("Completed Task Future Due", result.Data[0].Title);
    }

    [Fact]
    public async Task GetCompletedTasks_ShouldHandleTasksWithPastDates()
    {
        ToDoTask taskWithPastDate = new()
        {
            Title = "Completed Task with Past Due Date",
            DueDate = DateTime.UtcNow.AddYears(-100),  // A very old task
            IsCompleted = true
        };

        await _dbContext.AddAsync(taskWithPastDate);
        await _dbContext.SaveChangesAsync();

        ServiceResult<List<ToDoTask>> result = await _toDoService.GetCompletedTasks();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal("Completed Task with Past Due Date", result.Data[0].Title);  // Old tasks should still show up in completed tasks
    }

    public async Task GetCompletedTasks_ShouldReturnCompletedTasks_WithSearchAndSort()
    {
        await _dbContext.Tasks.AddRangeAsync(new List<ToDoTask>
        {
            new() { Title = "Task 1", IsCompleted = true, DueDate = DateTime.UtcNow.AddDays(-5) },
            new() { Title = "Task 2", IsCompleted = true, DueDate = DateTime.UtcNow.AddDays(-1) },
            new() { Title = "Other Task", IsCompleted = false, DueDate = DateTime.UtcNow.AddDays(2) }
        });
        await _dbContext.SaveChangesAsync();

        var result = await _toDoService.GetCompletedTasks("Task", "DueDate", "desc");

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("Task 2", result.Data[0].Title);
    }

    [Fact]
    public async Task GetCompletedTasks_ShouldHandleNoCompletedTasks()
    {
        await _dbContext.Tasks.AddAsync(new ToDoTask { Title = "Incomplete Task", IsCompleted = false, DueDate = new DateTime() });
        await _dbContext.SaveChangesAsync();

        var result = await _toDoService.GetCompletedTasks(null, null, null);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);
    }
}