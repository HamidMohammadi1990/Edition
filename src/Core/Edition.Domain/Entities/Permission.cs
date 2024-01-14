namespace Edition.Domain.Entities;

public class Permission : IEntity<PermissionType>
{
    public PermissionType Id { get; set; }
    public string Title { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string? NameSpace { get; set; }
    public PermissionType? ParentId { get; set; }
    public PermissionLevelType LevelTypeId { get; set; }
    public int PriorityId { get; set; }
    public bool IsActive { get; set; }


    public List<Permission>? Parents { get; set; }
    public List<RolePermission>? RolePermissions { get; set; }
}