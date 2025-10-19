using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bimcer.TaskManagement.Service.DataAccess;

public class TaskManagementDbContextFactory : IDesignTimeDbContextFactory<TaskManagementDbContext>
{
    public TaskManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TaskManagementDbContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=BimcerTaskDb;Trusted_Connection=True;TrustServerCertificate=True");
        return new TaskManagementDbContext(optionsBuilder.Options);
    }
}

