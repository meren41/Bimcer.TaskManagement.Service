using Microsoft.Extensions.DependencyInjection;

namespace Bimcer.TaskManagement.Service.Core.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Hashing/JWT helper kayıtları vs burada olacak (ileri adım)
        return services;
    }
}
