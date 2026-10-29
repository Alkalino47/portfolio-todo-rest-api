using Microsoft.AspNetCore.Mvc;
using portfolio_todo_rest_api.Application.Common;
using portfolio_todo_rest_api.Application.Todos;

namespace portfolio_todo_rest_api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class TodoController(ITodoService todoService) : ControllerBase
{
    private readonly ITodoService _todoService = todoService;

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<TodoDTO>>> GetTodos(
        [FromQuery] TodoPaginationQuery paginationQuery,
        CancellationToken cancellationToken)
    {
        var todos = await _todoService.GetTodosAsync(paginationQuery, cancellationToken);

        return Ok(todos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoDTO>> GetTodo(int id, CancellationToken cancellationToken)
    {
        var todo = await _todoService.GetTodoAsync(id, cancellationToken);

        return this.ToActionResult(todo);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutTodo(int id, TodoForPutDTO todoForPutDTO, CancellationToken cancellationToken)
    {
        var todo = await _todoService.UpdateTodoAsync(id, todoForPutDTO, cancellationToken);
        if (!todo.Succeeded)
        {
            return this.ToActionResult(todo).Result!;
        }

        return Ok(todo.Value);
    }

    [HttpPost]
    public async Task<ActionResult<TodoDTO>> PostTodo(TodoForPostDTO todoForPostDTO, CancellationToken cancellationToken)
    {
        var todo = await _todoService.CreateTodoAsync(todoForPostDTO, cancellationToken);
        if (!todo.Succeeded || todo.Value == null)
        {
            return this.ToActionResult(todo);
        }

        return CreatedAtAction(
            nameof(GetTodo),
            new { id = todo.Value.TodoId },
            todo.Value);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTodo(int id, CancellationToken cancellationToken)
    {
        var deleted = await _todoService.DeleteTodoAsync(id, cancellationToken);

        return this.ToActionResult(deleted);
    }

    [HttpPatch("{id:int}/archive")]
    [ProducesResponseType(typeof(TodoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchTodoArchiveStatus(
        int id,
        TodoArchivePatchDTO archivePatchDTO,
        CancellationToken cancellationToken)
    {
        var todo = await _todoService.SetArchiveStatusAsync(id, archivePatchDTO, cancellationToken);
        if (!todo.Succeeded)
        {
            return this.ToActionResult(todo).Result!;
        }

        return Ok(todo.Value);
    }

    [HttpPatch("{id:int}/complete")]
    [ProducesResponseType(typeof(TodoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchTodoCompleteStatus(
        int id,
        TodoCompletePatchDTO completePatchDTO,
        CancellationToken cancellationToken)
    {
        var todo = await _todoService.SetCompleteStatusAsync(id, completePatchDTO, cancellationToken);
        if (!todo.Succeeded)
        {
            return this.ToActionResult(todo).Result!;
        }

        return Ok(todo.Value);
    }
}
