using portfolio_todo_rest_api.Application.Common;
using portfolio_todo_rest_api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace portfolio_todo_rest_api.Infrastructure.Persistence;

public partial class TodosDbContext : DbContext
{
    public TodosDbContext(DbContextOptions<TodosDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Todo> Todos { get; set; }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConcurrencyConflictException(
                "The record was modified by another request. Refresh the resource and retry.", exception);
        }
        catch (DbUpdateException exception)
        {
            throw new PersistenceConflictException(
                "The request could not be saved because it conflicts with the current database state.", exception);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.HasKey(e => e.CategoryId);

            entity.Property(e => e.CategoryId).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Todo>(entity =>
        {
            entity.HasKey(e => e.TodoId);

            entity.Property(e => e.DateArchived).HasColumnType("datetime");
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateModified).HasColumnType("datetime");
            entity.Property(e => e.DatetimeReminder).HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UserId).IsRequired();

            entity.HasOne<Category>()
                .WithMany(category => category.Todos)
                .HasForeignKey(todo => todo.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
