using portfolio_todo_rest_api.Application.Categories;
using portfolio_todo_rest_api.Application.Todos;
using portfolio_todo_rest_api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace portfolio_todo_rest_api.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TodosDBConn");

        services.AddDbContext<TodosDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
