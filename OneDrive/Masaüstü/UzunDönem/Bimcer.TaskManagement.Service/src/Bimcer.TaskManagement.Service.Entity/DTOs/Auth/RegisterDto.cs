namespace Bimcer.TaskManagement.Service.Entity.DTOs.Auth;

public class RegisterDto
{
    public string FirstName { get; set; } = default!;
    public string LastName  { get; set; } = default!;
    public string Email     { get; set; } = default!;
    public string Password  { get; set; } = default!;
}
