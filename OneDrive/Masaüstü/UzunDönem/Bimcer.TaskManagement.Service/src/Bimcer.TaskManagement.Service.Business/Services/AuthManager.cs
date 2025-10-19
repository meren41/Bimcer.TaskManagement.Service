using Bimcer.TaskManagement.Service.Business.Abstractions;
using Bimcer.TaskManagement.Service.Business.Rules;
using Bimcer.TaskManagement.Service.Contracts.Auth;
using Bimcer.TaskManagement.Service.Core.Abstractions;
using Bimcer.TaskManagement.Service.Core.Exceptions;
using Bimcer.TaskManagement.Service.Core.Jwt;
using Bimcer.TaskManagement.Service.Core.Security;
using Bimcer.TaskManagement.Service.DataAccess;
using Bimcer.TaskManagement.Service.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bimcer.TaskManagement.Service.Business.Services;

public sealed class AuthManager(
    TaskManagementDbContext db,
    JwtHelper jwt,
    UserBusinessRules userRules) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest r)
    {
        await userRules.EnsureEmailNotTakenAsync(r.Email);
        await userRules.EnsureUsernameNotTakenAsync(r.Username);

        var hash = HashingHelper.CreateHash(r.Password);
        var user = new User
        {
            Username = r.Username,
            Email = r.Email,
            PasswordHash = hash,
            Role = "User",
            CreatedAt = DateTimeOffset.UtcNow
        };
        await db.Users.AddAsync(user);

        // ⬇️ Refresh token üret & kaydet
        var (rt, rtExp) = jwt.CreateRefreshToken();
        await db.RefreshTokens.AddAsync(new RefreshToken
        {
            Token = rt,
            Expires = rtExp,
            UserId = user.Id
        });

        await db.SaveChangesAsync();

        var (at, atExp) = jwt.CreateAccessToken(user);
        return new AuthResponse
        {
            AccessToken = at,
            ExpiresAtUtc = atExp,
            RefreshToken = rt,
            User = new BasicUserDto { Id = user.Id, Username = user.Username, Email = user.Email, Role = user.Role }
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest r)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == r.Email)
                   ?? throw new BusinessException("Geçersiz kimlik bilgisi.");

        if (!HashingHelper.Verify(r.Password, user.PasswordHash))
            throw new BusinessException("Geçersiz kimlik bilgisi.");

        // ⬇️ (Opsiyonel) Eski aktif refresh token'ları iptal et (rotate on login)
        var oldActives = await db.RefreshTokens
            .Where(x => x.UserId == user.Id && !x.IsRevoked && x.Expires > DateTimeOffset.UtcNow)
            .ToListAsync();
        foreach (var t in oldActives) t.IsRevoked = true;

        // ⬇️ Yeni refresh token oluştur
        var (rt, rtExp) = jwt.CreateRefreshToken();
        await db.RefreshTokens.AddAsync(new RefreshToken
        {
            Token = rt,
            Expires = rtExp,
            UserId = user.Id
        });

        await db.SaveChangesAsync();

        var (at, atExp) = jwt.CreateAccessToken(user);
        return new AuthResponse
        {
            AccessToken = at,
            ExpiresAtUtc = atExp,
            RefreshToken = rt,
            User = new BasicUserDto { Id = user.Id, Username = user.Username, Email = user.Email, Role = user.Role }
        };
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new BusinessException("Refresh token gerekli.");

        // ⬇️ Token'ı bul + user'la birlikte getir
        var token = await db.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

        if (token is null) throw new BusinessException("Geçersiz refresh token.");
        if (!token.IsActive) throw new BusinessException("Refresh token aktif değil (süresi dolmuş veya iptal edilmiş).");

        // ⬇️ Eski refresh token'ı iptal et (rotation)
        token.IsRevoked = true;

        // ⬇️ Yeni refresh token üret
        var (newRt, newRtExp) = jwt.CreateRefreshToken();
        await db.RefreshTokens.AddAsync(new RefreshToken
        {
            Token = newRt,
            Expires = newRtExp,
            UserId = token.UserId
        });

        // ⬇️ Yeni access token üret
        var (newAt, newAtExp) = jwt.CreateAccessToken(token.User);

        await db.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = newAt,
            ExpiresAtUtc = newAtExp,
            RefreshToken = newRt,
            User = new BasicUserDto
            {
                Id = token.User.Id,
                Username = token.User.Username,
                Email = token.User.Email,
                Role = token.User.Role
            }
        };
    }

    public async Task RevokeTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token gerekli.", nameof(token));

        var refreshToken = await db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        if (refreshToken is null)
            throw new BusinessException("Token bulunamadı.");

        refreshToken.IsRevoked = true;
        await db.SaveChangesAsync();
    }

    public async Task LogoutAsync(string userId, RefreshRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new Bimcer.TaskManagement.Service.Core.Exceptions.BusinessException("Refresh token gerekli.");

        var token = await db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == request.RefreshToken);
        if (token is null) return; // sessizce yok say (güvenlik için detay vermemek iyi bir pratik)

        // Token size mı ait?
        if (token.UserId != userId)
            throw new Bimcer.TaskManagement.Service.Core.Exceptions.BusinessException("Bu refresh token size ait değil.");

        // Zaten iptal edilmişse sorun değil
        if (!token.IsRevoked)
            token.IsRevoked = true;

        await db.SaveChangesAsync();
    }
}
