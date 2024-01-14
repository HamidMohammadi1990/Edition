namespace Edition.Domain.Entities;

public class User : IEntity
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public bool PhoneNumberConfirmed { get; set; }
    public string? PasswordHash { get; set; }
    public Gender? Gender { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public int AccessFailedCount { get; set; }
    public string SecurityStamp { get; set; } = null!;


    public List<UserRole>? UserRoles { get; set; }
}