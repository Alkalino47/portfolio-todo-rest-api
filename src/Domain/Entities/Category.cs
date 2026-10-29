namespace portfolio_todo_rest_api.Domain.Entities;

public class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<Todo> Todos { get; set; } = new List<Todo>();
}
