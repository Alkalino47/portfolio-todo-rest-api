using portfolio_todo_rest_api.Application.Common;
using portfolio_todo_rest_api.Domain.Entities;

namespace portfolio_todo_rest_api.Application.Todos;

public class TodoService(
    ITodoRepository todoRepository,
    TimeProvider timeProvider) : ITodoService
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<PaginatedResult<TodoDTO>> GetTodosAsync(TodoPaginationQuery paginationQuery, CancellationToken cancellationToken)
    {
        var skip = (paginationQuery.PageNumber - 1) * paginationQuery.PageSize;
        var totalCount = await _todoRepository.CountTodosAsync(paginationQuery, cancellationToken);
        var todos = await _todoRepository.GetTodosAsync(paginationQuery, skip, paginationQuery.PageSize, cancellationToken);

        return new PaginatedResult<TodoDTO>
        {
            Items = todos.Select(TodoToDTO).ToList(),
            Pagination = new PaginationMetadata
            {
                PageNumber = paginationQuery.PageNumber,
                PageSize = paginationQuery.PageSize,
                TotalCount = totalCount
            }
        };
    }

    public async Task<Result<TodoDTO>> GetTodoAsync(int id, CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetTodoAsync(id, cancellationToken);

        return todo == null
            ? Result<TodoDTO>.Failure(Errors.NotFound, $"Todo {id} was not found.")
            : Result<TodoDTO>.Success(TodoToDTO(todo));
    }

    public async Task<Result<TodoDTO>> CreateTodoAsync(TodoForPostDTO todoForPostDTO, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateReferencesAsync(todoForPostDTO.CategoryId, cancellationToken);
        if (!validationResult.Succeeded)
        {
            return Result<TodoDTO>.Failure(validationResult.ErrorCode!, validationResult.ErrorMessage!);
        }

        var now = GetUtcNow();

        var todo = new Todo
        {
            Name = todoForPostDTO.Name,
            Description = todoForPostDTO.Description,
            IsComplete = false,
            DatetimeReminder = todoForPostDTO.DatetimeReminder,
            CategoryId = todoForPostDTO.CategoryId,
            UserId = todoForPostDTO.UserId,
            DateCreated = now,
            DateModified = now,
            IsArchived = false,
            DateArchived = null
        };

        await _todoRepository.AddTodoAsync(todo, cancellationToken);
        await _todoRepository.SaveChangesAsync(cancellationToken);

        return Result<TodoDTO>.Success(TodoToDTO(todo));
    }

    public async Task<Result<TodoDTO>> UpdateTodoAsync(int id, TodoForPutDTO todoForPutDTO, CancellationToken cancellationToken)
    {
        if (id != todoForPutDTO.TodoId)
        {
            return Result<TodoDTO>.Failure(Errors.Validation, "Route todo id must match the request body TodoId.");
        }

        var validationResult = await ValidateReferencesAsync(todoForPutDTO.CategoryId, cancellationToken);
        if (!validationResult.Succeeded)
        {
            return Result<TodoDTO>.Failure(validationResult.ErrorCode!, validationResult.ErrorMessage!);
        }

        var todo = await _todoRepository.GetTodoForUpdateAsync(id, cancellationToken);
        if (todo == null)
        {
            return Result<TodoDTO>.Failure(Errors.NotFound, $"Todo {id} was not found.");
        }

        var now = GetUtcNow();

        todo.Name = todoForPutDTO.Name;
        todo.Description = todoForPutDTO.Description;
        todo.IsComplete = todoForPutDTO.IsComplete;
        todo.DatetimeReminder = todoForPutDTO.DatetimeReminder;
        todo.CategoryId = todoForPutDTO.CategoryId;
        todo.UserId = todoForPutDTO.UserId;
        todo.DateModified = now;
        todo.IsArchived = todoForPutDTO.IsArchived;
        todo.DateArchived = todo.IsArchived ? now : null;

        await _todoRepository.SaveChangesAsync(cancellationToken);

        return Result<TodoDTO>.Success(TodoToDTO(todo));
    }

    public async Task<Result> DeleteTodoAsync(int id, CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetTodoForUpdateAsync(id, cancellationToken);
        if (todo == null)
        {
            return Result.Failure(Errors.NotFound, $"Todo {id} was not found.");
        }

        _todoRepository.DeleteTodo(todo);
        await _todoRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<TodoDTO>> SetArchiveStatusAsync(
        int id,
        TodoArchivePatchDTO archivePatchDTO,
        CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetTodoForUpdateAsync(id, cancellationToken);
        if (todo == null)
        {
            return Result<TodoDTO>.Failure(Errors.NotFound, $"Todo {id} was not found.");
        }

        var now = GetUtcNow();

        todo.IsArchived = archivePatchDTO.IsArchived;
        todo.DateArchived = archivePatchDTO.IsArchived ? now : null;
        todo.DateModified = now;

        await _todoRepository.SaveChangesAsync(cancellationToken);

        return Result<TodoDTO>.Success(TodoToDTO(todo));
    }

    public async Task<Result<TodoDTO>> SetCompleteStatusAsync(
        int id,
        TodoCompletePatchDTO completePatchDTO,
        CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetTodoForUpdateAsync(id, cancellationToken);
        if (todo == null)
        {
            return Result<TodoDTO>.Failure(Errors.NotFound, $"Todo {id} was not found.");
        }

        todo.IsComplete = completePatchDTO.IsComplete;
        todo.DateModified = GetUtcNow();

        await _todoRepository.SaveChangesAsync(cancellationToken);

        return Result<TodoDTO>.Success(TodoToDTO(todo));
    }

    private DateTime GetUtcNow()
    {
        return _timeProvider.GetUtcNow().UtcDateTime;
    }

    private static TodoDTO TodoToDTO(Todo todo) =>
        new()
        {
            TodoId = todo.TodoId,
            Name = todo.Name,
            Description = todo.Description,
            IsComplete = todo.IsComplete,
            DatetimeReminder = todo.DatetimeReminder,
            CategoryId = todo.CategoryId,
            UserId = todo.UserId,
            DateCreated = todo.DateCreated,
            DateModified = todo.DateModified,
            IsArchived = todo.IsArchived,
            DateArchived = todo.DateArchived
        };

    private async Task<Result> ValidateReferencesAsync(int categoryId, CancellationToken cancellationToken)
    {
        if (!await _todoRepository.CategoryExistsAsync(categoryId, cancellationToken))
        {
            return Result.Failure(Errors.Validation, $"Category {categoryId} does not exist.");
        }

        return Result.Success();
    }
}
