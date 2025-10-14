namespace Bimcer.TaskManagement.Service.Entity.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public string Token { get; set; } = default!;
    public DateTimeOffset Expires { get; set; }
    public bool IsRevoked { get; set; } = false;
}
