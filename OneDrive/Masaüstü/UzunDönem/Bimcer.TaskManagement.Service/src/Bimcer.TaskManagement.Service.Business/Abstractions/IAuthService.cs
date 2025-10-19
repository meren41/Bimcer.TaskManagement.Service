using Bimcer.TaskManagement.Service.Contracts.Auth;

namespace Bimcer.TaskManagement.Service.Business.Abstractions;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshAsync(RefreshRequest request);
    Task LogoutAsync(string userId, RefreshRequest request);
    Task RevokeTokenAsync(string token);
}
