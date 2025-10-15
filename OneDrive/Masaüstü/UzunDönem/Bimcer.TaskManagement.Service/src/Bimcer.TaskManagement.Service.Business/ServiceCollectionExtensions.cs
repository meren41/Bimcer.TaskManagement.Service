using Microsoft.Extensions.DependencyInjection;

namespace Bimcer.TaskManagement.Service.Business;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        // services.AddScoped<IAuthService, AuthManager>();
        // services.AddScoped<ITaskService, TaskManager>();
        // services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        return services;
    }
}
