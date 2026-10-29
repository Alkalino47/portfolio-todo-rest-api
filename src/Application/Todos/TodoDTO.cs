namespace portfolio_todo_rest_api.Application.Todos;

public class TodoDTO
{
    public int TodoId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool IsComplete { get; set; }

    public DateTime? DatetimeReminder { get; set; }

    public int CategoryId { get; set; }

    public int UserId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }

    public bool IsArchived { get; set; }

    public DateTime? DateArchived { get; set; }
}
