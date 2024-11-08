using API.Data;
using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Tests;
public class ToDoServiceGeneralTests : IDisposable
{
    private readonly ToDoDbContext _dbContext;
    private readonly ToDoService _toDoService;

    public ToDoServiceGeneralTests()
    {
        var options = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase(databaseName: "ToDoDbTest_Generic")
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
    public async Task Create_ShouldAddTask_WhenValid()
    {
        ToDoTask task = new()
        {
            Title = "Test Task",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        await _toDoService.Create(task);
        var result = await _toDoService.GetOne(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(task.Title, result.Data?.Title);
        Assert.Contains(_dbContext.Tasks, t => t.Id == result.Data?.Id);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenTitleIsEmpty()
    {
        ToDoTask task = new()
        {
            Title = "",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        ServiceResult<ToDoTask> result = await _toDoService.Create(task);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenTitleIsNull()
    {
        ToDoTask task = new()
        {
            Title = null!,
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        ServiceResult<ToDoTask> result = await _toDoService.Create(task);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenDueDateIsInPast()
    {
        ToDoTask task = new()
        {
            Title = "Test Task with Past Due Date",
            DueDate = DateTime.UtcNow.AddDays(-1),
            IsCompleted = false
        };

        ServiceResult<ToDoTask> result = await _toDoService.Create(task);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetOne_ShouldReturnTask_WhenIdExists()
    {
        ToDoTask task = new()
        {
            Title = "Existing Task",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        await _dbContext.AddAsync(task);
        await _dbContext.SaveChangesAsync();

        ServiceResult<ToDoTask> result = await _toDoService.GetOne(task.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(task.Id, result.Data?.Id);
    }

    [Fact]
    public async Task GetOne_ShouldReturnNotFound_WhenIdDoesNotExist()
    {
        ServiceResult<ToDoTask> result = await _toDoService.GetOne(999);

        Assert.False(result.IsSuccess);
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
    public async Task Update_ShouldModifyExistingTask_WhenTaskExists()
    {
        ToDoTask task = new()
        {
            Title = "Old Title",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        await _dbContext.AddAsync(task);
        await _dbContext.SaveChangesAsync();

        task.Title = "Updated Title";
        task.IsCompleted = true;
        ServiceResult<ToDoTask> result = await _toDoService.Update(task);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Title", result.Data?.Title);
        Assert.True(result.Data?.IsCompleted);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenTaskDoesNotExist()
    {
        ToDoTask nonExistentTask = new()
        {
            Id = 999,
            Title = "Nonexistent Task",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        ServiceResult<ToDoTask> result = await _toDoService.Update(nonExistentTask);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_ShouldRemoveTask_WhenIdExists()
    {
        ToDoTask task = new()
        {
            Title = "Task to be deleted",
            DueDate = DateTime.UtcNow.AddDays(1),
            IsCompleted = false
        };

        await _dbContext.AddAsync(task);
        await _dbContext.SaveChangesAsync();

        ServiceResult<string> deleteResult = await _toDoService.Delete(task.Id);
        Assert.True(deleteResult.IsSuccess);

        ServiceResult<ToDoTask> result = await _toDoService.GetOne(task.Id);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_ShouldDoNothing_WhenIdDoesNotExist()
    {
#pragma warning disable
        int initialCount = (await _toDoService.GetAll()).Data.Count;

        ServiceResult<string> deleteResult = await _toDoService.Delete(999);
        Assert.False(deleteResult.IsSuccess);

        var finalCount = (await _toDoService.GetAll()).Data.Count;
        Assert.Equal(initialCount, finalCount);
    }
}