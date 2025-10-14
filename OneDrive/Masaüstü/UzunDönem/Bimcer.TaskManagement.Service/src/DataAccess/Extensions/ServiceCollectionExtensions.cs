using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bimcer.TaskManagement.Service.DataAccess.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration? configuration = null)
    {
        // Bağlantı adı: DefaultConnection
        services.AddDbContext<TaskManagementDbContext>(opt =>
            opt.UseSqlServer(configuration?.GetConnectionString("DefaultConnection") 
                             ?? "Server=.;Database=BimcerTaskDb;Trusted_Connection=True;TrustServerCertificate=True"));

        // Generic/Concrete repository kayıtları burada (ileri adım)
        return services;
    }
}
