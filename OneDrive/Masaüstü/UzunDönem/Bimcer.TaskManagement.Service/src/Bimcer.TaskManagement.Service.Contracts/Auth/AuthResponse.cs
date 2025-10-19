namespace Bimcer.TaskManagement.Service.Contracts.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public BasicUserDto User { get; set; } = null!;
}

