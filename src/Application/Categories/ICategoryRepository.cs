using portfolio_todo_rest_api.Domain.Entities;

namespace portfolio_todo_rest_api.Application.Categories;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken);

    Task<Category?> GetCategoryAsync(int id, CancellationToken cancellationToken);

    Task<Category?> GetCategoryForUpdateAsync(int id, CancellationToken cancellationToken);

    Task<bool> CategoryExistsAsync(int id, CancellationToken cancellationToken);

    Task AddCategoryAsync(Category category, CancellationToken cancellationToken);

    void DeleteCategory(Category category);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
