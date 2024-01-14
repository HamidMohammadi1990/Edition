using Newtonsoft.Json;
using Edition.Common.Enums;
using Edition.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Edition.Common.Extensions;

namespace Edition.WebFramework.Api;

public class ApiResult(bool isSuccess, OperationStatusCode statusCode, params string[] messages)
{
    public bool IsSuccess { get; set; } = isSuccess;
    public OperationStatusCode StatusCode { get; set; } = statusCode;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<OperationError> Messages { get; set; } =
        messages?.Select(x => new OperationError(x ?? statusCode.ToDisplay())).ToList() ?? [];

    #region Implicit Operators
    public static implicit operator ApiResult(OkResult result)
    {
        return new ApiResult(true, OperationStatusCode.OK);
    }

    public static implicit operator ApiResult(BadRequestResult result)
    {
        return new ApiResult(false, OperationStatusCode.BadRequest);
    }

    public static implicit operator ApiResult(BadRequestObjectResult result)
    {
        string[] messages = [];
        if (!string.IsNullOrEmpty(result?.Value?.ToString()))
            messages = [result.Value.ToString()];
        if (result?.Value is not null && result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            errorMessages = errorMessages.Where(x => !string.IsNullOrEmpty(x));
            messages = [.. errorMessages];
        }
        return new ApiResult(false, OperationStatusCode.BadRequest, messages);
    }


    public static implicit operator ApiResult(ContentResult result)
    {
        return new ApiResult(true, OperationStatusCode.OK, result.Content);
    }

    public static implicit operator ApiResult(NotFoundResult result)
    {
        return new ApiResult(false, OperationStatusCode.NotFound);
    }

    public static implicit operator ApiResult(OperationResult result)
    {
        return new ApiResult(result.IsSuccess, result.Status, result.Message);
    }
    #endregion
}

public class ApiResult<TData>(bool isSuccess, OperationStatusCode statusCode, TData? data, params string[] messages)
    : ApiResult(isSuccess, statusCode, messages)
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TData? Data { get; set; } = data;

    #region Implicit Operators
    public static implicit operator ApiResult<TData>(TData data)
    {
        return new ApiResult<TData>(true, OperationStatusCode.OK, data);
    }

    public static implicit operator ApiResult<TData>(OkResult result)
    {
        return new ApiResult<TData>(true, OperationStatusCode.OK, default(TData));
    }

    public static implicit operator ApiResult<TData>(OkObjectResult result)
    {
        return new ApiResult<TData>(true, OperationStatusCode.OK, (TData)result.Value);
    }

    public static implicit operator ApiResult<TData>(BadRequestResult result)
    {
        return new ApiResult<TData>(false, OperationStatusCode.BadRequest, default(TData));
    }

    public static implicit operator ApiResult<TData>(BadRequestObjectResult result)
    {
        string[] messages = [];
        if (!string.IsNullOrEmpty(result?.Value?.ToString()))
            messages = [result.Value.ToString()];
        if (result?.Value is not null && result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            errorMessages = errorMessages.Where(x => !string.IsNullOrEmpty(x));
            messages = [.. errorMessages];
        }
        return new ApiResult<TData>(false, OperationStatusCode.BadRequest, default(TData), messages);
    }


    public static implicit operator ApiResult<TData>(ContentResult result)
    {
        return new ApiResult<TData>(true, OperationStatusCode.OK, default(TData), result.Content);
    }

    public static implicit operator ApiResult<TData>(NotFoundResult result)
    {
        return new ApiResult<TData>(false, OperationStatusCode.NotFound, default(TData));
    }

    public static implicit operator ApiResult<TData>(NotFoundObjectResult result)
    {
        return new ApiResult<TData>(false, OperationStatusCode.NotFound, (TData)result.Value);
    }

    public static implicit operator ApiResult<TData>(OperationResult<TData> result)
    {
        return new ApiResult<TData>(result.IsSuccess, result.Status, result.Result);
    }
    #endregion
}