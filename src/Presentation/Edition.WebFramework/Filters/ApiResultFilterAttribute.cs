using Edition.Common.Enums;
using Edition.WebFramework.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Edition.WebFramework.Filters;

public class ApiResultFilterAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is OkObjectResult okObjectResult)
        {
            var apiResult = new ApiResult<object>(true, OperationStatusCode.OK, okObjectResult?.Value);
            context.Result = new JsonResult(apiResult) { StatusCode = okObjectResult.StatusCode };
        }
        else if (context.Result is OkResult okResult)
        {
            var apiResult = new ApiResult(true, OperationStatusCode.OK);
            context.Result = new JsonResult(apiResult) { StatusCode = okResult.StatusCode };
        }
        else if (context.Result is ObjectResult badRequestObjectResult && badRequestObjectResult.StatusCode == 400)
        {
            string[] messages = [];
            switch (badRequestObjectResult.Value)
            {
                case ValidationProblemDetails validationProblemDetails:
                    messages = validationProblemDetails.Errors.SelectMany(p => p.Value).Distinct().ToArray();
                    break;
                case SerializableError errors:
                    messages = errors.SelectMany(p => (string[])p.Value).Distinct().ToArray();
                    break;
                case var value when value != null && value is not ProblemDetails:
                    messages = [badRequestObjectResult?.Value?.ToString()];
                    break;
            }
            var apiResult = new ApiResult(false, OperationStatusCode.BadRequest, messages);
            context.Result = new JsonResult(apiResult) { StatusCode = badRequestObjectResult?.StatusCode };
        }
        else if (context.Result is ObjectResult notFoundObjectResult && notFoundObjectResult.StatusCode == 404)
        {
            string? message = null;
            if (notFoundObjectResult.Value != null && !(notFoundObjectResult.Value is ProblemDetails))
                message = notFoundObjectResult.Value.ToString();

            var apiResult = new ApiResult(false, OperationStatusCode.NotFound, message);
            context.Result = new JsonResult(apiResult) { StatusCode = notFoundObjectResult.StatusCode };
        }
        else if (context.Result is ContentResult contentResult)
        {
            var apiResult = new ApiResult(true, OperationStatusCode.OK, contentResult.Content);
            context.Result = new JsonResult(apiResult) { StatusCode = contentResult.StatusCode };
        }
        else if (context.Result is ObjectResult objectResult && objectResult.StatusCode == null
            && objectResult.Value is not ApiResult)
        {
            var apiResult = new ApiResult<object>(true, OperationStatusCode.OK, objectResult.Value);
            context.Result = new JsonResult(apiResult) { StatusCode = objectResult.StatusCode };
        }
        base.OnResultExecuting(context);
    }
}