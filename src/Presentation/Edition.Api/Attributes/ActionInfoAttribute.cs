using Edition.Domain.Enums;

namespace Edition.Api.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ActionInfoAttribute(PermissionType permissionType) : Attribute
{
    public PermissionType PermissionType { get; } = permissionType;
}