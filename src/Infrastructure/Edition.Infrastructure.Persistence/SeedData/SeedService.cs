using Edition.Domain.Enums;
using Edition.Domain.Common;
using Edition.Domain.Entities;
using Edition.Common.Utilities;
using Edition.Application.Models;
using Microsoft.EntityFrameworkCore;
using Edition.Application.Common.Contracts;

namespace Edition.Infrastructure.Persistence.SeedData;

public class SeedService(IEditionContext context) : ISeedService
{
    public async Task SeedDataAsync(List<DynamicPermission> dynamicPermissions)
    {
        #region Roles
        var roles = new List<Role>
        {
            new() { Title = "مدیر", IsActive = true },
            new() { Title = "فروشنده", IsActive = true }
        };
        var existsRoles = await context
                           .Role
                           .AsNoTracking()
                           .ToListAsync();

        roles = roles.Where(role => !existsRoles
                     .Any(x => x.Title == role.Title))
                     .ToList();
        if (roles.Count != 0)
        {
            context.Role.AddRange(roles);
            await context.SaveAllChangesAsync();
        }
        #endregion

        #region Permissions
        var adminRoleId =
            roles.Count != 0
            ? roles.First().Id
            : existsRoles.Single(x => x.Title == "مدیر" || x.Title.Contains("مدیر")).Id;

        var priority = 1;
        foreach (var dynamicPermission in dynamicPermissions)
        {
            var existsTabPermission = await context
                             .Permission
                             .FirstOrDefaultAsync(
                              x => x.Title == dynamicPermission.Name
                              && x.ParentId == PermissionType.Product);
            var tabPermission = existsTabPermission ?? new Permission
            {
                Id = dynamicPermission.Controllers[0].GroupType,
                Url = "",
                Title = dynamicPermission.Name,
                ParentId = PermissionType.Product,
                IsActive = true,
                PriorityId = priority,
                LevelTypeId = PermissionLevelType.Tab
            };
            tabPermission.Parents = [];

            var versionOfControllers =
                 dynamicPermission.Controllers
                                  .GroupBy(x => GetControllerName(x.FullName))
                                  .ToList();
            foreach (var controller in versionOfControllers)
            {
                var actions = controller.ToList()
                                        .SelectMany(x => x.Actions)
                                        .DistinctBy(x => x.Name)
                                        .ToList();

                var firstController = controller.First();
                var existsPagePermission =
                    await context.Permission
                    .SingleOrDefaultAsync(x => x.Title == firstController.Name
                                          && x.NameSpace == firstController.FullName);
                var pagePermission = existsPagePermission ?? new Permission
                {
                    Id = firstController.Type,
                    ParentId = tabPermission.Id,
                    Url = firstController.Url,
                    Title = firstController.Name,
                    IsActive = true,
                    PriorityId = ++priority,
                    NameSpace = firstController.FullName,
                    LevelTypeId = PermissionLevelType.Page,
                };
                pagePermission.Parents = [];

                foreach (var action in actions)
                {
                    var isExistsAction =
                        await context.Permission
                                     .AnyAsync(x => x.Title == action.Name
                                               && x.NameSpace == action.FullNames.FirstOrDefault());
                    if (isExistsAction)
                        continue;

                    var pagePermissionAction = new Permission
                    {
                        Id = action.Type,
                        ParentId = pagePermission.Id,
                        Url = action.Url,
                        Title = action.Name,
                        IsActive = true,
                        NameSpace = action.FullNames.FirstOrDefault(),
                        PriorityId = ++priority,
                        LevelTypeId = PermissionLevelType.Action
                    };
                    if (!pagePermission.Parents
                                       .Any(x => x.Title == pagePermissionAction.Title
                                            && x.NameSpace == pagePermissionAction.NameSpace))
                        pagePermission.Parents.Add(pagePermissionAction);
                }
                if (!tabPermission.Parents
                    .Any(x => x.Title == pagePermission.Title && x.NameSpace == pagePermission.NameSpace))
                    tabPermission.Parents.Add(pagePermission);
            }
            if (existsTabPermission is null)
                context.Permission.Add(tabPermission);
            else
                context.Permission.Update(tabPermission);

            priority++;
        }
        await context.SaveAllChangesAsync();
        #endregion

        #region Users
        List<User> users = [
            new()
            {
                PasswordHash = SecurityUtility.GetSha256Hash("admin"),
                UserName = "Admin",
                IsActive = true,
                FirstName = "Hamid",
                LastName = "Mohammadnian",
                UserRoles = [new() { RoleId = adminRoleId }],
            },
            new()
            {
                PasswordHash = SecurityUtility.GetSha256Hash("writer"),
                UserName = "Admin2",
                IsActive = true,
                FirstName = "Hossein",
                LastName = "Ojaq",
                UserRoles = [new() { RoleId = adminRoleId }]
            }];
        foreach (var user in users)
        {
            if (!await context.User.AnyAsync(x => x.UserName == user.UserName))
                context.User.Add(user);
        }
        await context.SaveAllChangesAsync();
        #endregion

        #region Role Permissions
        var permissionIds =
                        context.ChangeTracker
                       .Entries<Permission>()
                       .SelectMany(x => x.Properties)
                       .Where(x => x.Metadata.Name == nameof(IEntity.Id))
                       .Select(x => x.CurrentValue)
                       .ToList();

        if (permissionIds.Count != 0)
        {
            var rolePermissionIds =
                                context.RolePermission
                               .Where(x => permissionIds.Contains(x.Id))
                               .AsNoTracking()
                               .Select(x => x.Id)
                               .ToList();

            permissionIds = permissionIds
                            .Where(x => !rolePermissionIds
                            .Contains(Convert.ToInt32(x)))
                            .ToList();
            if (permissionIds.Count != 0)
            {
                var rolePermissionsForAdmin =
                        permissionIds.Select(x => new RolePermission
                        {
                            RoleId = adminRoleId,
                            PermissionId = (PermissionType)x!
                        }).ToList();
                context.RolePermission.AddRange(rolePermissionsForAdmin);
                await context.SaveAllChangesAsync();
            }
        }
        #endregion


        static string GetControllerName(string fullName)
            => fullName.Split(".").Last();
    }
}