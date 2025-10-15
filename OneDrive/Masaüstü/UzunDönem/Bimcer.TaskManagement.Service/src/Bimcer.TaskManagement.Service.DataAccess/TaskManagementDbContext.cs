using Bimcer.TaskManagement.Service.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bimcer.TaskManagement.Service.DataAccess;

public class TaskManagementDbContext : DbContext
{
    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("Users");
            b.HasKey(x => x.Id);

            b.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            b.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            b.Property(x => x.Email).IsRequired().HasMaxLength(200);
            b.HasIndex(x => x.Email).IsUnique();

            b.Property(x => x.PasswordHash).IsRequired();
            b.Property(x => x.PasswordSalt);

            b.Property(x => x.IsActive).HasDefaultValue(true);
            b.Property(x => x.CreatedAtUtc)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        // TaskItem
        modelBuilder.Entity<TaskItem>(b =>
        {
            b.ToTable("Tasks");
            b.HasKey(x => x.Id);

            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Description);
            b.Property(x => x.Status).IsRequired().HasMaxLength(32);
            b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

            b.HasOne(x => x.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.ToTable("RefreshTokens");
            b.HasKey(x => x.Id);

            b.Property(x => x.Token).IsRequired();
            b.HasIndex(x => x.Token).IsUnique();

            b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

            b.HasOne(x => x.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
