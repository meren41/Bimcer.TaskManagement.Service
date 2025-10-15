namespace Bimcer.TaskManagement.Service.Entity.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAtUtc { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = default!;

    public bool IsActive => RevokedAtUtc == null && DateTime.UtcNow <= ExpiresAtUtc;
}
