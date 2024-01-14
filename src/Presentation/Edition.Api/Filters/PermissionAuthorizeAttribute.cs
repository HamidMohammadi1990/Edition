using MediatR;
using Edition.Api.Attributes;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Edition.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Edition.Application.Features.Permissions.Queries.HasPermission;

namespace Edition.Api.Filters;

/// <summary>
/// Check User Has Permission
/// </summary>
public class PermissionAuthorizeAttribute
    (ISender sender)
    : AuthorizeAttribute, IAuthorizationFilter
{    
    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        var actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
        var isAllowAnonymous =
            actionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();

        if (!isAllowAnonymous)
        {
            if (!(context.HttpContext.User.Identity?.IsAuthenticated ?? false))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var claimsIdentity = context.HttpContext.User.Identity as ClaimsIdentity;
            if (claimsIdentity!.Claims?.Any() != true)
            {
                context.Result = new NotFoundObjectResult("درخواست نامعتبر");
                return;
            }

            var userId = claimsIdentity.GetUserId<int>();
            var permissionAttribute = 
                actionDescriptor
                .MethodInfo
                .GetAttribute<ActionInfoAttribute>();
            var hasPermissionResult =
                sender.Send(new HasPermissionRequestQuery(userId, permissionAttribute!.PermissionType))
                      .Result;

            if (!hasPermissionResult.HasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}