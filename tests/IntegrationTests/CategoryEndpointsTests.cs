using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using portfolio_todo_rest_api.Application.Categories;

namespace IntegrationTests;

public sealed class CategoryEndpointsTests : IDisposable
{
    private readonly TestApiFactory _factory;
    private readonly HttpClient _client;

    public CategoryEndpointsTests()
    {
        _factory = new TestApiFactory();
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            JwtTokenFactory.CreateToken());
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task GetCategories_ReturnsCategories()
    {
        var categories = await _client.GetFromJsonAsync<IReadOnlyList<CategoryDTO>>("/api/v1/category");

        Assert.NotNull(categories);
        Assert.Contains(categories, category => category.CategoryId == 1001);
    }

    [Fact]
    public async Task PostCategory_WithDuplicateId_ReturnsConflictProblemDetails()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/category",
            new CategoryForPostDTO { CategoryId = 1001, Name = "Duplicate" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}
