using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using portfolio_todo_rest_api.Application.Common;
using portfolio_todo_rest_api.Application.Todos;

namespace IntegrationTests;

public sealed class TodoEndpointsTests : IDisposable
{
    private readonly TestApiFactory _factory;
    private readonly HttpClient _client;

    public TodoEndpointsTests()
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
    public async Task GetTodos_WithoutBearerToken_ReturnsUnauthorized()
    {
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/todo?pageNumber=1&pageSize=10");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTodos_ReturnsPaginatedTodos()
    {
        var result = await _client.GetFromJsonAsync<PaginatedResult<TodoDTO>>(
            "/api/v1/todo?pageNumber=1&pageSize=10&isComplete=false");

        Assert.NotNull(result);
        Assert.Equal(1, result.Pagination.PageNumber);
        Assert.Equal(10, result.Pagination.PageSize);
        Assert.True(result.Pagination.TotalCount >= 1);
        Assert.Contains(result.Items, todo => todo.TodoId == 1);
    }

    [Fact]
    public async Task PostTodo_WithValidPayload_ReturnsCreatedTodo()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/todo",
            new TodoForPostDTO
            {
                Name = "Integration todo",
                Description = "Created by the integration test.",
                CategoryId = 1001,
                UserId = 1
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var todo = await response.Content.ReadFromJsonAsync<TodoDTO>();
        Assert.NotNull(todo);
        Assert.Equal("Integration todo", todo.Name);
        Assert.False(todo.IsComplete);
    }

    [Fact]
    public async Task PatchTodoComplete_WhenTodoExists_ReturnsUpdatedTodo()
    {
        var response = await _client.PatchAsJsonAsync(
            "/api/v1/todo/1/complete",
            new TodoCompletePatchDTO { IsComplete = true });

        response.EnsureSuccessStatusCode();
        var todo = await response.Content.ReadFromJsonAsync<TodoDTO>();
        Assert.NotNull(todo);
        Assert.True(todo.IsComplete);
    }

    [Fact]
    public async Task GetTodo_WhenMissing_ReturnsNotFoundProblemDetails()
    {
        var response = await _client.GetAsync("/api/v1/todo/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}
