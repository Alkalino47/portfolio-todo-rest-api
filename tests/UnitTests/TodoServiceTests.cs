using portfolio_todo_rest_api.Application.Common;
using portfolio_todo_rest_api.Application.Todos;
using portfolio_todo_rest_api.Domain.Entities;

namespace UnitTests;

public class TodoServiceTests
{
    private static readonly DateTimeOffset UtcNow = new(2026, 6, 26, 12, 30, 0, TimeSpan.Zero);

    [Fact]
    public async Task CreateTodoAsync_WhenReferencesExist_CreatesTodoWithDefaults()
    {
        var todoRepository = new FakeTodoRepository();
        todoRepository.AddCategory(1001);
        var service = new TodoService(todoRepository, new TestTimeProvider(UtcNow));

        var result = await service.CreateTodoAsync(
            new TodoForPostDTO
            {
                Name = "Prepare notes",
                Description = "Write project status notes.",
                DatetimeReminder = null,
                CategoryId = 1001,
                UserId = 1
            },
            CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal("Prepare notes", result.Value.Name);
        Assert.False(result.Value.IsComplete);
        Assert.False(result.Value.IsArchived);
        Assert.Equal(UtcNow.UtcDateTime, result.Value.DateCreated);
        Assert.Equal(UtcNow.UtcDateTime, result.Value.DateModified);
        Assert.Equal(1, todoRepository.SaveChangesCount);
    }

    [Fact]
    public async Task CreateTodoAsync_WhenCategoryDoesNotExist_ReturnsValidationFailure()
    {
        var todoRepository = new FakeTodoRepository();
        var service = new TodoService(todoRepository, new TestTimeProvider(UtcNow));

        var result = await service.CreateTodoAsync(
            new TodoForPostDTO
            {
                Name = "Prepare notes",
                Description = "Write project status notes.",
                CategoryId = 9999,
                UserId = 1
            },
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(Errors.Validation, result.ErrorCode);
        Assert.Equal(0, todoRepository.SaveChangesCount);
    }

    [Fact]
    public async Task SetArchiveStatusAsync_WhenTodoExists_UpdatesArchiveFields()
    {
        var todoRepository = new FakeTodoRepository();
        todoRepository.AddExistingTodo(new Todo
        {
            TodoId = 1,
            Name = "Old task",
            Description = "Task to archive",
            CategoryId = 1001,
            UserId = 1,
            DateCreated = UtcNow.AddDays(-1).UtcDateTime,
            DateModified = UtcNow.AddDays(-1).UtcDateTime
        });
        var service = new TodoService(todoRepository, new TestTimeProvider(UtcNow));

        var result = await service.SetArchiveStatusAsync(
            1,
            new TodoArchivePatchDTO { IsArchived = true },
            CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.IsArchived);
        Assert.Equal(UtcNow.UtcDateTime, result.Value.DateArchived);
        Assert.Equal(UtcNow.UtcDateTime, result.Value.DateModified);
        Assert.Equal(1, todoRepository.SaveChangesCount);
    }

    [Fact]
    public async Task GetTodosAsync_ReturnsPaginationMetadata()
    {
        var todoRepository = new FakeTodoRepository();
        todoRepository.AddExistingTodo(CreateTodo(1));
        todoRepository.AddExistingTodo(CreateTodo(2));
        todoRepository.AddExistingTodo(CreateTodo(3));
        var service = new TodoService(todoRepository, new TestTimeProvider(UtcNow));

        var result = await service.GetTodosAsync(
            new TodoPaginationQuery { PageNumber = 2, PageSize = 2 },
            CancellationToken.None);

        Assert.Equal(2, result.Pagination.PageNumber);
        Assert.Equal(2, result.Pagination.PageSize);
        Assert.Equal(3, result.Pagination.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal(3, result.Items[0].TodoId);
    }

    private static Todo CreateTodo(int id) =>
        new()
        {
            TodoId = id,
            Name = $"Todo {id}",
            Description = $"Description {id}",
            CategoryId = 1001,
            UserId = 1,
            DateCreated = UtcNow.UtcDateTime,
            DateModified = UtcNow.UtcDateTime
        };
}
