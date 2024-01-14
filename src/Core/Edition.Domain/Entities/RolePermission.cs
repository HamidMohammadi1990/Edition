namespace Edition.Domain.Entities;

public class RolePermission
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public PermissionType PermissionId { get; set; }

    
    public Permission? Permission { get; set; }
    public Role? Role { get; set; }
}