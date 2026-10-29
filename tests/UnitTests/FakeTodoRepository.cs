using portfolio_todo_rest_api.Application.Todos;
using portfolio_todo_rest_api.Domain.Entities;

namespace UnitTests;

internal sealed class FakeTodoRepository : ITodoRepository
{
    private readonly List<Todo> _todos = [];
    private readonly HashSet<int> _categoryIds = [];

    public int SaveChangesCount { get; private set; }

    public IReadOnlyList<Todo> Todos => _todos;

    public void AddExistingTodo(Todo todo) => _todos.Add(todo);

    public void AddCategory(int categoryId) => _categoryIds.Add(categoryId);

    public Task<IReadOnlyList<Todo>> GetTodosAsync(
        TodoPaginationQuery query,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<Todo>>(_todos.Skip(skip).Take(take).ToList());
    }

    public Task<int> CountTodosAsync(TodoPaginationQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(_todos.Count);
    }

    public Task<Todo?> GetTodoAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_todos.SingleOrDefault(todo => todo.TodoId == id));
    }

    public Task<Todo?> GetTodoForUpdateAsync(int id, CancellationToken cancellationToken)
    {
        return GetTodoAsync(id, cancellationToken);
    }

    public Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_categoryIds.Contains(categoryId));
    }

    public Task AddTodoAsync(Todo todo, CancellationToken cancellationToken)
    {
        todo.TodoId = _todos.Count == 0 ? 1 : _todos.Max(existingTodo => existingTodo.TodoId) + 1;
        _todos.Add(todo);

        return Task.CompletedTask;
    }

    public void DeleteTodo(Todo todo)
    {
        _todos.Remove(todo);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;

        return Task.CompletedTask;
    }
}
