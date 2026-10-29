using System.ComponentModel.DataAnnotations;

namespace portfolio_todo_rest_api.Application.Categories;

public class CategoryForPutDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than 0")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(80, ErrorMessage = "Name cannot be longer than 80 characters")]
    public string Name { get; set; } = null!;
}
