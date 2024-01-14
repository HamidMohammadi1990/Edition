using System.Reflection;
using Edition.Domain.Enums;
using Edition.Api.Attributes;
using Microsoft.AspNetCore.Mvc;
using Edition.Common.Extensions;
using Edition.Application.Models;
using Microsoft.AspNetCore.Authorization;

namespace Edition.Api.Modules;

/// <summary>
/// Get Controllers And Methods For Insert To DB At Runtime
/// </summary>
public static class PermissionModule
{
    public static List<DynamicPermission> GetPermissions()
    {
        var result = new List<DynamicPermission>();
        var assembly = Assembly.GetExecutingAssembly();
        var controllers = assembly.GetExportedTypes()
            .Where(type => type.IsPublic
            && type.IsClass
            && !type.IsAbstract
            && type.IsSubclassOf(typeof(ControllerBase))
            && type.GetCustomAttribute<AllowAnonymousAttribute>() is null)
            .Select(controller => new
            {
                ControllerFullName = controller.FullName,
                ControllerName = GetControllerName(controller),
                PermissionType = GetControllerPermissionType(controller),
                ControllerUrl = GetControllerUrl(controller.FullName),
                ControllerGroupType = GetControllerGroupType(controller),
                Actions = controller.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                    .Where(method => method.IsPublic
                    && method.GetCustomAttribute<NonActionAttribute>() is null
                    && method.GetCustomAttribute<AllowAnonymousAttribute>() is null)
                    .Select(method => new
                    {
                        ActionUrl = GetActionUrl(controller.FullName, method),
                        ActionName = GetActionName(method),
                        ActionFullName = controller.FullName + "." + method.Name,
                        PermissionType = GetActionPermissionType(method)
                    }).ToList(),
            }).ToList();

        foreach (var page in controllers.GroupBy(p => p.ControllerGroupType))
        {
            var permission = new DynamicPermission { Name = page.Key.ToDisplay() };
            foreach (var controller in page)
            {
                var permissionController = new PermissionController
                {
                    Url = controller.ControllerUrl.ToLower(),
                    Name = controller.ControllerName,
                    FullName = controller.ControllerFullName ?? "NULL",
                    Type = controller.PermissionType,
                    GroupType = page.Key
                };
                foreach (var action in controller.Actions.GroupBy(p => p.ActionName))
                {
                    var permissionAction = new PermissionAction
                    {
                        Url = action.First().ActionUrl,
                        Name = action.Key,
                        Type = action.First()!.PermissionType,                        
                        FullNames = action.Select(p => p.ActionFullName).Distinct().ToList()
                    };
                    permissionController.Actions.Add(permissionAction);
                }
                permission.Controllers.Add(permissionController);
            }
            result.Add(permission);
        }
        return result;
    }


    private static string GetControllerName(Type type)
    {
        var attribute = type.GetCustomAttribute<ControllerInfoAttribute>();
        return attribute?.PermissionType.ToDisplay() ?? type.Name.TrimEnd("Controller");
    }

    private static PermissionType GetControllerPermissionType(Type type)
    {
        var attribute = type.GetCustomAttribute<ControllerInfoAttribute>();
        return attribute!.PermissionType;
    }

    private static PermissionType GetControllerGroupType(Type type)
    {
        var attribute = type.GetCustomAttribute<ControllerInfoAttribute>();
        return attribute?.GroupType ?? GetControllerPermissionType(type);
    }

    private static string GetControllerUrl(string controllerFullName)
    {
        var splitData = controllerFullName.Split(".");
        var controllerName = splitData[4].Replace("Controller", "").ToLower();
        var url = $"/api/{splitData[3]}/{controllerName}";
        return url;
    }

    private static string GetActionName(MethodInfo methodInfo)
    {
        var actionInfo = methodInfo.GetCustomAttribute<ActionInfoAttribute>();
        if (actionInfo?.PermissionType != null)
            return actionInfo.PermissionType.ToDisplay();

        return methodInfo.Name switch
        {
            "Index" or "Detail" or "Details" or "Search" or "Read" => "مشاهده",
            "Add" or "Insert" or "Create" => "ثبت",
            "Edit" or "Put" or "Update" => "ویرایش",
            "Delete" or "Remove" => "حذف",
            _ => methodInfo.Name,
        };
    }

    private static string GetActionUrl(string controllerFullName, MethodInfo methodInfo)
    {
        var controllerUrl = GetControllerUrl(controllerFullName);

        var httpGetAttribute = methodInfo.GetCustomAttribute<HttpGetAttribute>();
        if (httpGetAttribute is not null)
            return $"{controllerUrl}/{httpGetAttribute.Template}";


        var httpPostAttribute = methodInfo.GetCustomAttribute<HttpPostAttribute>();
        if (httpPostAttribute is not null)
            return $"{controllerUrl}/{httpPostAttribute.Template}";


        var httpPutAttribute = methodInfo.GetCustomAttribute<HttpPutAttribute>();
        if (httpPutAttribute is not null)
            return $"{controllerUrl}/{httpPutAttribute.Template}";


        var httpDeleteAttribute = methodInfo.GetCustomAttribute<HttpDeleteAttribute>();
        if (httpDeleteAttribute is not null)
            return $"{controllerUrl}/{httpDeleteAttribute.Template}";


        return $"{controllerUrl}/{methodInfo.Name.ToLower()}";
    }

    private static PermissionType GetActionPermissionType(MethodInfo methodInfo)
    {
        var actionInfo = methodInfo.GetCustomAttribute<ActionInfoAttribute>();
        return actionInfo!.PermissionType;
    }
}