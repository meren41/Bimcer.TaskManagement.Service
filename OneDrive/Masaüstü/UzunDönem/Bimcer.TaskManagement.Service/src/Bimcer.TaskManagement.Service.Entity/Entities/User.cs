namespace Bimcer.TaskManagement.Service.Entity.Entities;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Username { get; set; } = default!; // unique
    public string Email { get; set; } = default!; // unique
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = "User"; // "User" or "Admin"

    public DateTimeOffset CreatedAt { get; set; }

    // Navs
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
