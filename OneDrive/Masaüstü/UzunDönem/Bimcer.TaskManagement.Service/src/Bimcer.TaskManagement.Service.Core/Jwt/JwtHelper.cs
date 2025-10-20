using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Bimcer.TaskManagement.Service.Entity.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Bimcer.TaskManagement.Service.Core.Jwt;

public sealed class JwtHelper
{
    private readonly JwtOptions _opt;
    private readonly byte[] _key;

    public JwtHelper(JwtOptions options)
    {
        _opt = options ?? throw new ArgumentNullException(nameof(options));
        _key = Encoding.UTF8.GetBytes(_opt.SecurityKey);
    }

    public (string Token, DateTime ExpiresAtUtc) CreateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("username", user.Username),
            new("role", user.Role)
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_opt.AccessTokenMinutes);

        var jwt = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(jwt), expires);
    }

    /// Rastgele, kriptografik olarak güçlü bir refresh token üretir 
    public (string Token, DateTime ExpiresAtUtc) CreateRefreshToken()
    {
        // 64 bayt -> Base64 ~ 88 char. URL-safe istiyorsan Convert.ToBase64String yerine Base64UrlEncoder de kullanılabilir.
        var bytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(bytes);
        var expires = DateTime.UtcNow.AddDays(_opt.RefreshTokenDays);
        return (token, expires);
    }

    ///  Access token doğrulama (isteğe bağlı yardımcı).
    public ClaimsPrincipal? ValidateAccessToken(string token, bool validateLifetime = true)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = validateLifetime,
                ValidIssuer = _opt.Issuer,
                ValidAudience = _opt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(_key),
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
