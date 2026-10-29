using portfolio_todo_rest_api.Application.Common;
using portfolio_todo_rest_api.Domain.Entities;

namespace portfolio_todo_rest_api.Application.Categories;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<IReadOnlyList<CategoryDTO>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);

        return categories.Select(CategoryToDTO).ToList();
    }

    public async Task<Result<CategoryDTO>> GetCategoryAsync(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetCategoryAsync(id, cancellationToken);

        return category == null
            ? Result<CategoryDTO>.Failure(Errors.NotFound, $"Category {id} was not found.")
            : Result<CategoryDTO>.Success(CategoryToDTO(category));
    }

    public async Task<Result<CategoryDTO>> CreateCategoryAsync(
        CategoryForPostDTO categoryForPostDTO,
        CancellationToken cancellationToken)
    {
        if (await _categoryRepository.CategoryExistsAsync(categoryForPostDTO.CategoryId, cancellationToken))
        {
            return Result<CategoryDTO>.Failure(
                Errors.Conflict,
                $"Category {categoryForPostDTO.CategoryId} already exists.");
        }

        var category = new Category
        {
            CategoryId = categoryForPostDTO.CategoryId,
            Name = categoryForPostDTO.Name
        };

        await _categoryRepository.AddCategoryAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return Result<CategoryDTO>.Success(CategoryToDTO(category));
    }

    public async Task<Result<CategoryDTO>> UpdateCategoryAsync(
        int id,
        CategoryForPutDTO categoryForPutDTO,
        CancellationToken cancellationToken)
    {
        if (id != categoryForPutDTO.CategoryId)
        {
            return Result<CategoryDTO>.Failure(
                Errors.Validation,
                "Route category id must match the request body CategoryId.");
        }

        var category = await _categoryRepository.GetCategoryForUpdateAsync(id, cancellationToken);
        if (category == null)
        {
            return Result<CategoryDTO>.Failure(Errors.NotFound, $"Category {id} was not found.");
        }

        category.Name = categoryForPutDTO.Name;

        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return Result<CategoryDTO>.Success(CategoryToDTO(category));
    }

    public async Task<Result> DeleteCategoryAsync(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetCategoryForUpdateAsync(id, cancellationToken);
        if (category == null)
        {
            return Result.Failure(Errors.NotFound, $"Category {id} was not found.");
        }

        _categoryRepository.DeleteCategory(category);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static CategoryDTO CategoryToDTO(Category category) =>
        new()
        {
            CategoryId = category.CategoryId,
            Name = category.Name
        };
}
