using Microsoft.Extensions.DependencyInjection;

namespace Bimcer.TaskManagement.Service.Business.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        // Manager/Validator kayıtları burada (ileri adım)
        return services;
    }
}
