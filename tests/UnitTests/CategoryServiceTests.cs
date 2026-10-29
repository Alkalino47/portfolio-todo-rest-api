using portfolio_todo_rest_api.Application.Categories;
using portfolio_todo_rest_api.Application.Common;
using portfolio_todo_rest_api.Domain.Entities;

namespace UnitTests;

public class CategoryServiceTests
{
    [Fact]
    public async Task CreateCategoryAsync_WhenCategoryAlreadyExists_ReturnsConflict()
    {
        var repository = new FakeCategoryRepository();
        repository.AddExistingCategory(new Category { CategoryId = 1001, Name = "Work" });
        var service = new CategoryService(repository);

        var result = await service.CreateCategoryAsync(
            new CategoryForPostDTO { CategoryId = 1001, Name = "Duplicate" },
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(Errors.Conflict, result.ErrorCode);
        Assert.Equal(0, repository.SaveChangesCount);
    }
}
