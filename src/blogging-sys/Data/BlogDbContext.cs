using BloggingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BloggingSystem.Data;

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("posts");
            entity.Property(p => p.Description).HasColumnType("TEXT");

            // belongsTo User via nullable UserId; null the FK when the user is removed.
            entity.HasOne(p => p.User)
                  .WithMany(u => u.Posts)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    // Mirror Laravel's automatic created_at / updated_at timestamps.
    public override int SaveChanges()
    {
        ApplyTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyTimestamps()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not Post && entry.Entity is not User)
                continue;

            if (entry.State == EntityState.Added)
            {
                SetIfExists(entry, "CreatedAt", now);
                SetIfExists(entry, "UpdatedAt", now);
            }
            else if (entry.State == EntityState.Modified)
            {
                SetIfExists(entry, "UpdatedAt", now);
            }
        }
    }

    private static void SetIfExists(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, string property, DateTime value)
    {
        var member = entry.Metadata.FindProperty(property);
        if (member is not null)
            entry.Property(property).CurrentValue = value;
    }
}
