using API.Data;
using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace API.Tests;
public class ToDoServiceSpecificTests : IDisposable
{
    private readonly ToDoDbContext _dbContext;
    private readonly ToDoService _toDoService;

    public ToDoServiceSpecificTests()
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