using portfolio_todo_rest_api.Application.Categories;
using portfolio_todo_rest_api.Application.Common;

namespace IntegrationTests;

internal sealed class FakeCategoryService(TestDataStore store) : ICategoryService
{
    private readonly TestDataStore _store = store;

    public Task<IReadOnlyList<CategoryDTO>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<CategoryDTO>>(_store.Categories);
    }

    public Task<Result<CategoryDTO>> GetCategoryAsync(int id, CancellationToken cancellationToken)
    {
        var category = _store.Categories.SingleOrDefault(category => category.CategoryId == id);

        return Task.FromResult(category == null
            ? Result<CategoryDTO>.Failure(Errors.NotFound, $"Category {id} was not found.")
            : Result<CategoryDTO>.Success(category));
    }

    public Task<Result<CategoryDTO>> CreateCategoryAsync(
        CategoryForPostDTO categoryForPostDTO,
        CancellationToken cancellationToken)
    {
        if (_store.Categories.Any(category => category.CategoryId == categoryForPostDTO.CategoryId))
        {
            return Task.FromResult(Result<CategoryDTO>.Failure(
                Errors.Conflict,
                $"Category {categoryForPostDTO.CategoryId} already exists."));
        }

        var category = new CategoryDTO
        {
            CategoryId = categoryForPostDTO.CategoryId,
            Name = categoryForPostDTO.Name
        };
        _store.Categories.Add(category);

        return Task.FromResult(Result<CategoryDTO>.Success(category));
    }

    public Task<Result<CategoryDTO>> UpdateCategoryAsync(
        int id,
        CategoryForPutDTO categoryForPutDTO,
        CancellationToken cancellationToken)
    {
        if (id != categoryForPutDTO.CategoryId)
        {
            return Task.FromResult(Result<CategoryDTO>.Failure(
                Errors.Validation,
                "Route category id must match the request body CategoryId."));
        }

        var category = _store.Categories.SingleOrDefault(existingCategory => existingCategory.CategoryId == id);
        if (category == null)
        {
            return Task.FromResult(Result<CategoryDTO>.Failure(Errors.NotFound, $"Category {id} was not found."));
        }

        category.Name = categoryForPutDTO.Name;

        return Task.FromResult(Result<CategoryDTO>.Success(category));
    }

    public Task<Result> DeleteCategoryAsync(int id, CancellationToken cancellationToken)
    {
        var category = _store.Categories.SingleOrDefault(existingCategory => existingCategory.CategoryId == id);
        if (category == null)
        {
            return Task.FromResult(Result.Failure(Errors.NotFound, $"Category {id} was not found."));
        }

        _store.Categories.Remove(category);

        return Task.FromResult(Result.Success());
    }
}
