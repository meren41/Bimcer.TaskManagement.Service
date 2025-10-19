using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Bimcer.TaskManagement.Service.Business.Abstractions;
using Bimcer.TaskManagement.Service.Business.Services;
using Bimcer.TaskManagement.Service.Business.Rules;
using Bimcer.TaskManagement.Service.Core.Jwt;

namespace Bimcer.TaskManagement.Service.Business;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        // JWT Options - Manual mapping
        var jwtSection = configuration.GetSection("Jwt");
        var jwtOptions = new JwtOptions
        {
            SecurityKey = jwtSection["SecurityKey"] ?? throw new InvalidOperationException("Jwt:SecurityKey is missing."),
            Issuer = jwtSection["Issuer"] ?? "TaskManagementService",
            Audience = jwtSection["Audience"] ?? "TaskManagementClient",
            AccessTokenMinutes = int.TryParse(jwtSection["AccessTokenMinutes"], out var atm) ? atm : 60,
            RefreshTokenDays = int.TryParse(jwtSection["RefreshTokenDays"], out var rtd) ? rtd : 7
        };

        // helpers
        services.AddSingleton(new JwtHelper(jwtOptions));

        // business rules
        services.AddScoped<UserBusinessRules>();

        // services
        services.AddScoped<IAuthService, AuthManager>();
        services.AddScoped<ITaskService, TaskManager>();

        return services;
    }
}
