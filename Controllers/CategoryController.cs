using portfolio_todo_rest_api.Application.Categories;
using Microsoft.AspNetCore.Mvc;

namespace portfolio_todo_rest_api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetCategoriesAsync(cancellationToken);

        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDTO>> GetCategory(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetCategoryAsync(id, cancellationToken);

        return this.ToActionResult(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDTO>> PostCategory(
        CategoryForPostDTO categoryForPostDTO,
        CancellationToken cancellationToken)
    {
        var category = await _categoryService.CreateCategoryAsync(categoryForPostDTO, cancellationToken);
        if (!category.Succeeded || category.Value == null)
        {
            return this.ToActionResult(category);
        }

        return CreatedAtAction(
            nameof(GetCategory),
            new { id = category.Value.CategoryId },
            category.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutCategory(
        int id,
        CategoryForPutDTO categoryForPutDTO,
        CancellationToken cancellationToken)
    {
        var category = await _categoryService.UpdateCategoryAsync(id, categoryForPutDTO, cancellationToken);
        if (!category.Succeeded)
        {
            return this.ToActionResult(category).Result!;
        }

        return Ok(category.Value);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
    {
        var deleted = await _categoryService.DeleteCategoryAsync(id, cancellationToken);

        return this.ToActionResult(deleted);
    }
}
