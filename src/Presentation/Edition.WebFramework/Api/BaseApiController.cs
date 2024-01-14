using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Edition.WebFramework.Filters;

namespace Edition.WebFramework.Api;

[ApiController]
[ApiResultFilter]
[Route("api/v{version:apiVersion}/[controller]")]
public class BaseApiController : ControllerBase
{
    protected string Token { get; set; }

    public BaseApiController()
    {
        Token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);
    }
}