using portfolio_todo_rest_api.Application.Categories;
using portfolio_todo_rest_api.Application.Todos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace portfolio_todo_rest_api.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<ITodoService, TodoService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}
