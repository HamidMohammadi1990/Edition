using MediatR;
using Asp.Versioning;
using Edition.Domain.Enums;
using Edition.Api.Attributes;
using Edition.WebFramework.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edition.Application.Models.Services;
using Edition.Application.Services.Contracts;
using Edition.Application.Features.Users.Queries.GetUserInfo;
using Edition.Application.Features.Users.Commands.CreateUser;
using Edition.Application.Features.Users.Commands.SignInUser;
using Edition.Application.Features.Users.Commands.SignOutUser;
using Edition.Application.Features.Users.Commands.RegisterUser;
using Edition.Application.Features.Users.Queries.UserNameCheck;

namespace Edition.Api.Controllers.v1;

/// <summary>
/// Management user account
/// </summary>
[ApiVersion("1")]
[ControllerInfo(PermissionType.ManageUsers, PermissionType.ManageUsersGroup)]
[AllowAnonymous]
public class AccountController
    (ISender mediator, IAccountingService accountingService)
    : BaseApiController
{
    [HttpPost("sign-in")]
    public async Task<ActionResult> SignInAsync([FromForm] SignInUserCommand request)
        => new JsonResult(await mediator.Send(request));

    [Authorize]
    [HttpGet("sign-out")]
    public async Task SignOut()
        => await mediator.Send(new SignOutUserCommand(Token));

    [HttpPost("refresh-token")]
    public async Task<ApiResult<AccessTokenResponse>> RefreshTokenAsync(RefreshTokenRequestDto request)
        => await accountingService.RefreshTokenAsync(request);

    [HttpPost("register")]
    public async Task<ApiResult<bool>> RegisterAsync(RegisterUserCommand request)
        => await mediator.Send(request);

    [Authorize]
    [HttpPost("is-authenticated")]
    public ApiResult<bool> IsAuthenticated()
        => true;

    [HttpPost("user-check")]
    public async Task<ApiResult<UserNameCheckResponseDto>> UserNameCheck(UserNameCheckRequestQuery request)
         => await mediator.Send(request);

    [Authorize]
    [HttpGet("user-info")]
    [ActionInfo(PermissionType.GetUserById)]
    public async Task<ApiResult<GetUserInfoResponseDto?>> UserInfoAsync(GetUserInfoRequestQuery request)
        => await mediator.Send(request);

    [Authorize]
    [HttpPost("create")]
    [ActionInfo(PermissionType.CreateUser)]
    public async Task<ApiResult<int>> CreateUserAsync(CreateUserCommand request)
        => await mediator.Send(request);
}