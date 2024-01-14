using System.Net;
using Newtonsoft.Json;
using Edition.Common.Enums;
using Edition.WebFramework.Api;
using Microsoft.AspNetCore.Http;
using Edition.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Edition.WebFramework.Middlewares;

public static class CustomExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}

public class CustomExceptionHandlerMiddleware
    (RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger)
{    
    public async Task Invoke(HttpContext context)
    {
        string? message = null;
        HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
        OperationStatusCode apiStatusCode = OperationStatusCode.ServerError;

        try
        {
            await next(context);
        }
        catch (SecurityTokenExpiredException exception)
        {
            logger.LogError(exception, exception.Message);
            SetUnAuthorizeResponse(exception);
            await WriteToResponseAsync();
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogError(exception, exception.Message);
            SetUnAuthorizeResponse(exception);
            await WriteToResponseAsync();
        }
        catch (AppValidationException exception)
        {
            var result = new ApiResult(false, OperationStatusCode.ServerError, [.. exception.Errors]);
            var json = JsonConvert.SerializeObject(result);

            context.Response.StatusCode = (int)OperationStatusCode.ServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);
#if DEBUG
            var dic = new Dictionary<string, string>
            {
                ["Exception"] = exception.Message,
                ["StackTrace"] = exception.StackTrace ?? "No Stack Trace.",
            };
            message = JsonConvert.SerializeObject(dic);
#endif
            await WriteToResponseAsync();
        }

        async Task WriteToResponseAsync()
        {
            if (context.Response.HasStarted)
                throw new InvalidOperationException("The response has already started, the http status code middleware will not be executed.");

            var result = new ApiResult(false, apiStatusCode, message);
            var json = JsonConvert.SerializeObject(result);

            context.Response.StatusCode = (int)httpStatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }

        void SetUnAuthorizeResponse(Exception exception)
        {
            httpStatusCode = HttpStatusCode.Unauthorized;
            apiStatusCode = OperationStatusCode.UnAuthorized;


#if DEBUG
            var dic = new Dictionary<string, string>
            {
                ["Exception"] = exception.Message,
                ["StackTrace"] = exception.StackTrace ?? "No Stack Trace."
            };
            if (exception is SecurityTokenExpiredException tokenException)
                dic.Add("Expires", tokenException.Expires.ToString());

            message = JsonConvert.SerializeObject(dic);
#endif            
        }
    }
}