namespace Bimcer.TaskManagement.Service.Entity.Entities;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!; // hash
    public string Role { get; set; } = "User";
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
