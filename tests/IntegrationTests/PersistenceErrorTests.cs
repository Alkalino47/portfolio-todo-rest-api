using System.Text.Json;
using portfolio_todo_rest_api;
using portfolio_todo_rest_api.Application.Common;
using portfolio_todo_rest_api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace IntegrationTests;

public class PersistenceErrorTests
{
    [Theory]
    [InlineData(true, "Concurrency conflict")]
    [InlineData(false, "Database update failed")]
    public async Task SaveChangesAsync_TranslatesEfConflict_AndHandlerReturnsConflictProblemDetails(
        bool isConcurrencyConflict,
        string expectedTitle)
    {
        Exception databaseException = isConcurrencyConflict
            ? new DbUpdateConcurrencyException("Internal database details")
            : new DbUpdateException("Internal database details");
        await using var context = CreateContext(databaseException);

        var exception = await Record.ExceptionAsync(() => context.SaveChangesAsync(CancellationToken.None));

        Assert.NotNull(exception);
        Assert.Equal(
            isConcurrencyConflict ? typeof(ConcurrencyConflictException) : typeof(PersistenceConflictException),
            exception.GetType());
        Assert.Same(databaseException, exception.InnerException);

        await using var services = new ServiceCollection()
            .AddLogging()
            .AddProblemDetails()
            .BuildServiceProvider();
        var httpContext = new DefaultHttpContext { RequestServices = services };
        await using var body = new MemoryStream();
        httpContext.Response.Body = body;
        var handler = new ApiExceptionHandler(
            services.GetRequiredService<IProblemDetailsService>(),
            NullLogger<ApiExceptionHandler>.Instance);

        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status409Conflict, httpContext.Response.StatusCode);
        Assert.StartsWith("application/problem+json", httpContext.Response.ContentType);
        body.Position = 0;
        using var problem = await JsonDocument.ParseAsync(body);
        Assert.Equal(expectedTitle, problem.RootElement.GetProperty("title").GetString());
        Assert.Equal(exception.Message, problem.RootElement.GetProperty("detail").GetString());
        Assert.Equal(httpContext.TraceIdentifier, problem.RootElement.GetProperty("traceId").GetString());
        Assert.DoesNotContain("Internal database details", problem.RootElement.GetRawText());
    }

    [Fact]
    public async Task SaveChangesAsync_DoesNotTranslateCancellation()
    {
        var cancellation = new OperationCanceledException();
        await using var context = CreateContext(cancellation);

        var exception = await Record.ExceptionAsync(() => context.SaveChangesAsync(CancellationToken.None));

        Assert.Same(cancellation, exception);
    }

    private static TodosDbContext CreateContext(Exception exception) => new(
        new DbContextOptionsBuilder<TodosDbContext>()
            .UseSqlServer("Server=localhost;Database=UnusedForInterceptorTests;Integrated Security=true")
            .AddInterceptors(new FailingSaveInterceptor(exception))
            .Options);

    private sealed class FailingSaveInterceptor(Exception exception) : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default) => throw exception;
    }
}
