using Bimcer.TaskManagement.Service.Core.Abstractions;
using Bimcer.TaskManagement.Service.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bimcer.TaskManagement.Service.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("Default")
                     ?? throw new InvalidOperationException("ConnectionStrings:Default not found.");

        services.AddDbContext<TaskManagementDbContext>(opt =>
            opt.UseSqlServer(connStr));

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<RefreshTokenRepository>();

        return services;
    }
}
