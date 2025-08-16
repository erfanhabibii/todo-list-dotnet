using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Todo.Core.Entities;
using Todo.Infrastructure.Auth;

namespace Todo.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TodoItem>(e =>
        {
            e.ToTable("todos");
            e.HasKey(t => t.Id);
            e.Property(t => t.Title).IsRequired().HasMaxLength(200);
            e.Property(t => t.IsDone).HasDefaultValue(false);
            e.Property(t => t.CreatedAtUtc).HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
            
            e.Property(t => t.OwnerId).IsRequired().HasMaxLength(100);
            e.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(t => t.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(t => new { t.OwnerId, t.IsDone, t.CreatedAtUtc });
        });
    }
}
