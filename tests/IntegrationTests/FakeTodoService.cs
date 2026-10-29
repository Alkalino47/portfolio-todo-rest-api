using portfolio_todo_rest_api.Application.Common;
using portfolio_todo_rest_api.Application.Todos;

namespace IntegrationTests;

internal sealed class FakeTodoService(TestDataStore store) : ITodoService
{
    private readonly TestDataStore _store = store;

    public Task<PaginatedResult<TodoDTO>> GetTodosAsync(
        TodoPaginationQuery paginationQuery,
        CancellationToken cancellationToken)
    {
        var todos = _store.Todos.AsEnumerable();

        if (paginationQuery.IsComplete.HasValue)
        {
            todos = todos.Where(todo => todo.IsComplete == paginationQuery.IsComplete.Value);
        }

        if (paginationQuery.IsArchived.HasValue)
        {
            todos = todos.Where(todo => todo.IsArchived == paginationQuery.IsArchived.Value);
        }

        if (paginationQuery.CategoryId.HasValue)
        {
            todos = todos.Where(todo => todo.CategoryId == paginationQuery.CategoryId.Value);
        }

        if (paginationQuery.UserId.HasValue)
        {
            todos = todos.Where(todo => todo.UserId == paginationQuery.UserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(paginationQuery.Search))
        {
            todos = todos.Where(todo =>
                todo.Name.Contains(paginationQuery.Search, StringComparison.OrdinalIgnoreCase) ||
                todo.Description.Contains(paginationQuery.Search, StringComparison.OrdinalIgnoreCase));
        }

        var filteredTodos = todos.ToList();
        var items = filteredTodos
            .Skip((paginationQuery.PageNumber - 1) * paginationQuery.PageSize)
            .Take(paginationQuery.PageSize)
            .ToList();

        return Task.FromResult(new PaginatedResult<TodoDTO>
        {
            Pagination = new PaginationMetadata
            {
                PageNumber = paginationQuery.PageNumber,
                PageSize = paginationQuery.PageSize,
                TotalCount = filteredTodos.Count
            },
            Items = items
        });
    }

    public Task<Result<TodoDTO>> GetTodoAsync(int id, CancellationToken cancellationToken)
    {
        var todo = _store.Todos.SingleOrDefault(todo => todo.TodoId == id);

        return Task.FromResult(todo == null
            ? Result<TodoDTO>.Failure(Errors.NotFound, $"Todo {id} was not found.")
            : Result<TodoDTO>.Success(todo));
    }

    public Task<Result<TodoDTO>> CreateTodoAsync(TodoForPostDTO todoForPostDTO, CancellationToken cancellationToken)
    {
        var now = _store.GetUtcNow();
        var todo = new TodoDTO
        {
            TodoId = _store.Todos.Count == 0 ? 1 : _store.Todos.Max(existingTodo => existingTodo.TodoId) + 1,
            Name = todoForPostDTO.Name,
            Description = todoForPostDTO.Description,
            DatetimeReminder = todoForPostDTO.DatetimeReminder,
            CategoryId = todoForPostDTO.CategoryId,
            UserId = todoForPostDTO.UserId,
            DateCreated = now,
            DateModified = now,
            IsComplete = false,
            IsArchived = false
        };

        _store.Todos.Add(todo);

        return Task.FromResult(Result<TodoDTO>.Success(todo));
    }

    public Task<Result<TodoDTO>> UpdateTodoAsync(
        int id,
        TodoForPutDTO todoForPutDTO,
        CancellationToken cancellationToken)
    {
        if (id != todoForPutDTO.TodoId)
        {
            return Task.FromResult(Result<TodoDTO>.Failure(
                Errors.Validation,
                "Route todo id must match the request body TodoId."));
        }

        var todo = _store.Todos.SingleOrDefault(existingTodo => existingTodo.TodoId == id);
        if (todo == null)
        {
            return Task.FromResult(Result<TodoDTO>.Failure(Errors.NotFound, $"Todo {id} was not found."));
        }

        todo.Name = todoForPutDTO.Name;
        todo.Description = todoForPutDTO.Description;
        todo.IsComplete = todoForPutDTO.IsComplete;
        todo.DatetimeReminder = todoForPutDTO.DatetimeReminder;
        todo.CategoryId = todoForPutDTO.CategoryId;
        todo.UserId = todoForPutDTO.UserId;
        todo.IsArchived = todoForPutDTO.IsArchived;
        todo.DateModified = _store.GetUtcNow();

        return Task.FromResult(Result<TodoDTO>.Success(todo));
    }

    public Task<Result> DeleteTodoAsync(int id, CancellationToken cancellationToken)
    {
        var todo = _store.Todos.SingleOrDefault(existingTodo => existingTodo.TodoId == id);
        if (todo == null)
        {
            return Task.FromResult(Result.Failure(Errors.NotFound, $"Todo {id} was not found."));
        }

        _store.Todos.Remove(todo);

        return Task.FromResult(Result.Success());
    }

    public Task<Result<TodoDTO>> SetArchiveStatusAsync(
        int id,
        TodoArchivePatchDTO archivePatchDTO,
        CancellationToken cancellationToken)
    {
        var todo = _store.Todos.SingleOrDefault(existingTodo => existingTodo.TodoId == id);
        if (todo == null)
        {
            return Task.FromResult(Result<TodoDTO>.Failure(Errors.NotFound, $"Todo {id} was not found."));
        }

        todo.IsArchived = archivePatchDTO.IsArchived;
        todo.DateArchived = archivePatchDTO.IsArchived ? _store.GetUtcNow() : null;
        todo.DateModified = _store.GetUtcNow();

        return Task.FromResult(Result<TodoDTO>.Success(todo));
    }

    public Task<Result<TodoDTO>> SetCompleteStatusAsync(
        int id,
        TodoCompletePatchDTO completePatchDTO,
        CancellationToken cancellationToken)
    {
        var todo = _store.Todos.SingleOrDefault(existingTodo => existingTodo.TodoId == id);
        if (todo == null)
        {
            return Task.FromResult(Result<TodoDTO>.Failure(Errors.NotFound, $"Todo {id} was not found."));
        }

        todo.IsComplete = completePatchDTO.IsComplete;
        todo.DateModified = _store.GetUtcNow();

        return Task.FromResult(Result<TodoDTO>.Success(todo));
    }
}
