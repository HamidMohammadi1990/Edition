namespace Edition.Domain.Entities;

public class RefreshToken(int userId, string jwtId, DateTime expiredDate)
{
    public int Id { get; set; }
    public string JwtId { get; set; } = jwtId;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpiredDate { get; set; } = expiredDate;
    public bool Used { get; set; } = false;
    public bool Invalidated { get; set; } = false;
    public int UserId { get; set; } = userId;


    public User User { get; set; } = null!;
}