using portfolio_todo_rest_api.Application.Todos;
using portfolio_todo_rest_api.Application.Common;
using portfolio_todo_rest_api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace portfolio_todo_rest_api.Infrastructure.Persistence;

public class TodoRepository(TodosDbContext context) : ITodoRepository
{
    private readonly TodosDbContext _context = context;

    public async Task<IReadOnlyList<Todo>> GetTodosAsync(
        TodoPaginationQuery query,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        return await ApplyTodoQuery(query, _context.Todos.AsNoTracking())
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountTodosAsync(TodoPaginationQuery query, CancellationToken cancellationToken)
    {
        return await ApplyTodoFilters(query, _context.Todos.AsNoTracking()).CountAsync(cancellationToken);
    }

    public async Task<Todo?> GetTodoAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Todos
            .AsNoTracking()
            .SingleOrDefaultAsync(todo => todo.TodoId == id, cancellationToken);
    }

    public async Task<Todo?> GetTodoForUpdateAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Todos.FindAsync([id], cancellationToken);
    }

    public async Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken)
    {
        return await _context.Categories.AnyAsync(category => category.CategoryId == categoryId, cancellationToken);
    }

    public async Task AddTodoAsync(Todo todo, CancellationToken cancellationToken)
    {
        await _context.Todos.AddAsync(todo, cancellationToken);
    }

    public void DeleteTodo(Todo todo)
    {
        _context.Todos.Remove(todo);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Todo> ApplyTodoQuery(TodoPaginationQuery query, IQueryable<Todo> todos)
    {
        todos = ApplyTodoFilters(query, todos);

        return query.SortBy?.Trim().ToLowerInvariant() switch
        {
            "name" => query.SortDirection == SortDirection.Desc
                ? todos.OrderByDescending(todo => todo.Name).ThenBy(todo => todo.TodoId)
                : todos.OrderBy(todo => todo.Name).ThenBy(todo => todo.TodoId),
            "datecreated" => query.SortDirection == SortDirection.Desc
                ? todos.OrderByDescending(todo => todo.DateCreated).ThenBy(todo => todo.TodoId)
                : todos.OrderBy(todo => todo.DateCreated).ThenBy(todo => todo.TodoId),
            "datemodified" => query.SortDirection == SortDirection.Desc
                ? todos.OrderByDescending(todo => todo.DateModified).ThenBy(todo => todo.TodoId)
                : todos.OrderBy(todo => todo.DateModified).ThenBy(todo => todo.TodoId),
            "iscomplete" => query.SortDirection == SortDirection.Desc
                ? todos.OrderByDescending(todo => todo.IsComplete).ThenBy(todo => todo.TodoId)
                : todos.OrderBy(todo => todo.IsComplete).ThenBy(todo => todo.TodoId),
            "isarchived" => query.SortDirection == SortDirection.Desc
                ? todos.OrderByDescending(todo => todo.IsArchived).ThenBy(todo => todo.TodoId)
                : todos.OrderBy(todo => todo.IsArchived).ThenBy(todo => todo.TodoId),
            "categoryid" => query.SortDirection == SortDirection.Desc
                ? todos.OrderByDescending(todo => todo.CategoryId).ThenBy(todo => todo.TodoId)
                : todos.OrderBy(todo => todo.CategoryId).ThenBy(todo => todo.TodoId),
            "userid" => query.SortDirection == SortDirection.Desc
                ? todos.OrderByDescending(todo => todo.UserId).ThenBy(todo => todo.TodoId)
                : todos.OrderBy(todo => todo.UserId).ThenBy(todo => todo.TodoId),
            _ => query.SortDirection == SortDirection.Desc
                ? todos.OrderByDescending(todo => todo.TodoId)
                : todos.OrderBy(todo => todo.TodoId)
        };
    }

    private static IQueryable<Todo> ApplyTodoFilters(TodoPaginationQuery query, IQueryable<Todo> todos)
    {
        if (query.IsComplete.HasValue)
        {
            todos = todos.Where(todo => todo.IsComplete == query.IsComplete.Value);
        }

        if (query.IsArchived.HasValue)
        {
            todos = todos.Where(todo => todo.IsArchived == query.IsArchived.Value);
        }

        if (query.CategoryId.HasValue)
        {
            todos = todos.Where(todo => todo.CategoryId == query.CategoryId.Value);
        }

        if (query.UserId.HasValue)
        {
            todos = todos.Where(todo => todo.UserId == query.UserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            todos = todos.Where(todo => todo.Name.Contains(search) || todo.Description.Contains(search));
        }

        return todos;
    }
}
