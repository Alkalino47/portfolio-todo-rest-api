using portfolio_todo_rest_api.Application.Common;

namespace portfolio_todo_rest_api.Application.Todos;

public interface ITodoService
{
    Task<PaginatedResult<TodoDTO>> GetTodosAsync(TodoPaginationQuery paginationQuery, CancellationToken cancellationToken);

    Task<Result<TodoDTO>> GetTodoAsync(int id, CancellationToken cancellationToken);

    Task<Result<TodoDTO>> CreateTodoAsync(TodoForPostDTO todoForPostDTO, CancellationToken cancellationToken);

    Task<Result<TodoDTO>> UpdateTodoAsync(int id, TodoForPutDTO todoForPutDTO, CancellationToken cancellationToken);

    Task<Result<TodoDTO>> SetArchiveStatusAsync(int id, TodoArchivePatchDTO archivePatchDTO, CancellationToken cancellationToken);

    Task<Result<TodoDTO>> SetCompleteStatusAsync(int id, TodoCompletePatchDTO completePatchDTO, CancellationToken cancellationToken);

    Task<Result> DeleteTodoAsync(int id, CancellationToken cancellationToken);
}
