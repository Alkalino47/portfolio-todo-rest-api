using System.ComponentModel.DataAnnotations;

namespace portfolio_todo_rest_api.Application.Todos;

public class TodoForPutDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "TodoId must be greater than 0")]
    public int TodoId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Description is required")]
    [MaxLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
    public string Description { get; set; } = null!;

    public bool IsComplete { get; set; }

    public DateTime? DatetimeReminder { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than 0")]
    public int CategoryId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "UserId must be greater than 0")]
    public int UserId { get; set; }

    public bool IsArchived { get; set; }
}
