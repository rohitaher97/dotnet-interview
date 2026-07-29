using System.IO;
using Xunit;
using Microsoft.Extensions.Configuration;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Tests;

public class TodoServiceTests : IDisposable
{
    private readonly string _databasePath;
    private readonly TodoService _service;

    public TodoServiceTests()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"todo-test-{Guid.NewGuid()}.db");
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:TodoDatabase"] = $"Data Source={_databasePath}"
            })
            .Build();

        _service = new TodoService(configuration);
        _service.InitializeDatabase();
    }

    [Fact]
    public void CreateTodo_ShouldReturnSavedTodo()
    {
        var todo = new Todo
        {
            Title = "Write tests",
            Description = "Add service unit tests",
            IsCompleted = false
        };

        var created = _service.CreateTodo(todo);

        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Write tests", created.Title);
        Assert.False(created.IsCompleted);
    }

    [Fact]
    public void GetTodoById_ShouldReturnTodo_WhenExists()
    {
        var created = _service.CreateTodo(new Todo { Title = "Item", Description = "Desc" });

        var found = _service.GetTodoById(created.Id);

        Assert.NotNull(found);
        Assert.Equal(created.Id, found!.Id);
    }

    [Fact]
    public void GetAllTodos_ShouldReturnEmptyCollection_WhenNoItems()
    {
        var todos = _service.GetAllTodos();

        Assert.Empty(todos);
    }

    [Fact]
    public void UpdateTodo_ShouldModifyExistingTodo()
    {
        var created = _service.CreateTodo(new Todo { Title = "Old", Description = "Old desc" });

        var updated = _service.UpdateTodo(created.Id, new Todo { Title = "New", Description = "New desc", IsCompleted = true });

        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated!.Id);
        Assert.Equal("New", updated.Title);
        Assert.True(updated.IsCompleted);
    }

    [Fact]
    public void DeleteTodo_ShouldReturnFalse_WhenNotFound()
    {
        var result = _service.DeleteTodo(9999);

        Assert.False(result);
    }

    [Fact]
    public void DeleteTodo_ShouldRemoveExistingTodo()
    {
        var created = _service.CreateTodo(new Todo { Title = "Delete me", Description = "temp" });

        var result = _service.DeleteTodo(created.Id);
        var found = _service.GetTodoById(created.Id);

        Assert.True(result);
        Assert.Null(found);
    }

    public void Dispose()
    {
        if (File.Exists(_databasePath))
        {
            try
            {
                File.Delete(_databasePath);
            }
            catch (IOException)
            {
                // The file may still be locked by SQLite; ignore cleanup failure.
            }
        }
    }
}
