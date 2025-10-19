namespace Bimcer.TaskManagement.Service.Core.Jwt;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string SecurityKey { get; init; } = default!;
    public int AccessTokenMinutes { get; init; } = 15;  // appsettings: Jwt:AccessTokenMinutes
    public int RefreshTokenDays { get; init; } = 7;      // appsettings: Jwt:RefreshTokenDays
}
