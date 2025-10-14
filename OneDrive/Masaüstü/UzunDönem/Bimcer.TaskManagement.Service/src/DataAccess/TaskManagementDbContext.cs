using Microsoft.EntityFrameworkCore;
using Bimcer.TaskManagement.Service.Entity.Entities;

namespace Bimcer.TaskManagement.Service.DataAccess;

public class TaskManagementDbContext : DbContext
{
    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options) : base(options) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Fluent API config’ler ileride burada
    }
}
