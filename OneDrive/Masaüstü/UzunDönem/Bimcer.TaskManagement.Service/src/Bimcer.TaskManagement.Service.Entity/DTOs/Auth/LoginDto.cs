namespace Bimcer.TaskManagement.Service.Entity.DTOs.Auth;

public class LoginDto
{
    public string Email    { get; set; } = default!;
    public string Password { get; set; } = default!;
}
