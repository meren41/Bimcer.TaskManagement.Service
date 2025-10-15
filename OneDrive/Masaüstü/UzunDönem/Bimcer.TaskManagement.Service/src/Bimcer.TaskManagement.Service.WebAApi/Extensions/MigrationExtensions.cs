using Bimcer.TaskManagement.Service.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Bimcer.TaskManagement.Service.WebAApi.Extensions;

public static class MigrationExtensions
{
    /// <summary>
    /// Uygulama açılışında bekleyen EF Core migration'larını uygular.
    /// Hata varsa loglar ve hatayı tekrar fırlatır.
    /// </summary>
    public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app, bool seed = false)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("EFMigrations");
        var db = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();

        try
        {
            await db.Database.MigrateAsync();
            logger.LogInformation("Database migration completed.");

            if (seed)
            {
                // Örnek tohumlama (isteğe bağlı):
                // if (!await db.Users.AnyAsync())
                // {
                //     db.Users.Add(new User { FirstName="Admin", LastName="User", Email="admin@bimcer.local", PasswordHash="...", PasswordSalt="..." });
                //     await db.SaveChangesAsync();
                // }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration failed.");
            throw; // kritik: açılışta patlatsın ki fark edilsin
        }

        return app;
    }
}
