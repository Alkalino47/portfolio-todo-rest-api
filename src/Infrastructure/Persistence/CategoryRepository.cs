using portfolio_todo_rest_api.Application.Categories;
using portfolio_todo_rest_api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace portfolio_todo_rest_api.Infrastructure.Persistence;

public class CategoryRepository(TodosDbContext context) : ICategoryRepository
{
    private readonly TodosDbContext _context = context;

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(category => category.CategoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetCategoryAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .SingleOrDefaultAsync(category => category.CategoryId == id, cancellationToken);
    }

    public async Task<Category?> GetCategoryForUpdateAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Categories.FindAsync([id], cancellationToken);
    }

    public async Task<bool> CategoryExistsAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Categories.AnyAsync(category => category.CategoryId == id, cancellationToken);
    }

    public async Task AddCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
    }

    public void DeleteCategory(Category category)
    {
        _context.Categories.Remove(category);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
