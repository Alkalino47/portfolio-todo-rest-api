using portfolio_todo_rest_api.Application.Common;

namespace portfolio_todo_rest_api.Application.Categories;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDTO>> GetCategoriesAsync(CancellationToken cancellationToken);

    Task<Result<CategoryDTO>> GetCategoryAsync(int id, CancellationToken cancellationToken);

    Task<Result<CategoryDTO>> CreateCategoryAsync(CategoryForPostDTO categoryForPostDTO, CancellationToken cancellationToken);

    Task<Result<CategoryDTO>> UpdateCategoryAsync(int id, CategoryForPutDTO categoryForPutDTO, CancellationToken cancellationToken);

    Task<Result> DeleteCategoryAsync(int id, CancellationToken cancellationToken);
}
