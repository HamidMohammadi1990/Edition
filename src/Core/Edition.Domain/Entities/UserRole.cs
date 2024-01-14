namespace Edition.Domain.Entities;

public class UserRole : IEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }


    public User? User { get; set; }
    public Role? Role { get; set; }   
}