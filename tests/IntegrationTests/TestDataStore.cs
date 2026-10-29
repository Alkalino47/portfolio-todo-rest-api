using portfolio_todo_rest_api.Application.Categories;
using portfolio_todo_rest_api.Application.Todos;

namespace IntegrationTests;

public sealed class TestDataStore
{
    private readonly DateTime _now = new(2026, 6, 26, 12, 0, 0, DateTimeKind.Utc);

    public TestDataStore()
    {
        Categories =
        [
            new CategoryDTO { CategoryId = 1001, Name = "Work" },
            new CategoryDTO { CategoryId = 1002, Name = "Personal" }
        ];

        Todos =
        [
            new TodoDTO
            {
                TodoId = 1,
                Name = "Seed todo",
                Description = "Existing todo for integration tests.",
                CategoryId = 1001,
                UserId = 1,
                DateCreated = _now,
                DateModified = _now,
                IsComplete = false,
                IsArchived = false
            }
        ];
    }

    public List<CategoryDTO> Categories { get; }

    public List<TodoDTO> Todos { get; }

    public DateTime GetUtcNow() => _now;
}
