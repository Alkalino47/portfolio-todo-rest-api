using portfolio_todo_rest_api.Application.Categories;
using portfolio_todo_rest_api.Domain.Entities;

namespace UnitTests;

internal sealed class FakeCategoryRepository : ICategoryRepository
{
    private readonly List<Category> _categories = [];

    public int SaveChangesCount { get; private set; }

    public IReadOnlyList<Category> Categories => _categories;

    public void AddExistingCategory(Category category) => _categories.Add(category);

    public Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<Category>>(_categories);
    }

    public Task<Category?> GetCategoryAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_categories.SingleOrDefault(category => category.CategoryId == id));
    }

    public Task<Category?> GetCategoryForUpdateAsync(int id, CancellationToken cancellationToken)
    {
        return GetCategoryAsync(id, cancellationToken);
    }

    public Task<bool> CategoryExistsAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_categories.Any(category => category.CategoryId == id));
    }

    public Task AddCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        _categories.Add(category);

        return Task.CompletedTask;
    }

    public void DeleteCategory(Category category)
    {
        _categories.Remove(category);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;

        return Task.CompletedTask;
    }
}
