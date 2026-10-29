using portfolio_todo_rest_api.Domain.Entities;

namespace portfolio_todo_rest_api.Application.Todos;

public interface ITodoRepository
{
    Task<IReadOnlyList<Todo>> GetTodosAsync(TodoPaginationQuery query, int skip, int take, CancellationToken cancellationToken);

    Task<int> CountTodosAsync(TodoPaginationQuery query, CancellationToken cancellationToken);

    Task<Todo?> GetTodoAsync(int id, CancellationToken cancellationToken);

    Task<Todo?> GetTodoForUpdateAsync(int id, CancellationToken cancellationToken);

    Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken);

    Task AddTodoAsync(Todo todo, CancellationToken cancellationToken);

    void DeleteTodo(Todo todo);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
