using Edition.Domain.Enums;

namespace Edition.Application.Models;

public class DynamicPermission
{
    public string Name { get; set; } = null!;
    public List<PermissionController> Controllers { get; set; } = [];
}

public class PermissionController
{
    public string Name { get; set; } = null!;
    public PermissionType Type { get; set; }
    public PermissionType GroupType { get; set; }
    public string FullName { get; set; } = null!;
    public string Url { get; set; } = null!;
    public List<PermissionAction> Actions { get; set; } = [];
}

public class PermissionAction
{
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
    public PermissionType Type { get; set; }
    public List<string> FullNames { get; set; }
}