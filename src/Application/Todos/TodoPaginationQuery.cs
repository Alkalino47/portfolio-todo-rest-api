using System.ComponentModel.DataAnnotations;
using portfolio_todo_rest_api.Application.Common;

namespace portfolio_todo_rest_api.Application.Todos;

public class TodoPaginationQuery
{
    [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be greater than 0")]
    public int PageNumber { get; init; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
    public int PageSize { get; init; } = 10;

    public bool? IsComplete { get; init; }

    public bool? IsArchived { get; init; }

    public int? CategoryId { get; init; }

    public int? UserId { get; init; }

    public string? Search { get; init; }

    public string? SortBy { get; init; }

    public SortDirection SortDirection { get; init; } = SortDirection.Asc;
}
