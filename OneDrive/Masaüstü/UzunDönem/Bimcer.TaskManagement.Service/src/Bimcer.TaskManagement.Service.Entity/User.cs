namespace Bimcer.TaskManagement.Service.Entity.Entities;

public class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = default!;
    public string LastName  { get; set; } = default!;
    public string Email     { get; set; } = default!; // unique

    // Şimdilik basit tutuyoruz: hash & salt string olarak saklanacak
    public string PasswordHash { get; set; } = default!;
    public string? PasswordSalt { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    // Navs
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
