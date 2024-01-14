namespace Edition.Domain.Entities;

public class Role : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool IsActive { get; set; }


    public List<RolePermission>? RolePermissions { get; set; }
    public List<UserRole>? UserRoles { get; set; }
}