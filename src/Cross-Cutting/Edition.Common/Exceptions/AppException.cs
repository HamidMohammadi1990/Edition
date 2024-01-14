using System.Net;
using Edition.Common.Enums;

namespace Edition.Common.Exceptions;

public class AppException : Exception
{
    public HttpStatusCode HttpStatusCode { get; set; }
    public OperationStatusCode ApiStatusCode { get; set; }
    public object AdditionalData { get; set; }

    public AppException()
        : this(OperationStatusCode.ServerError)
    {
    }

    public AppException(OperationStatusCode statusCode)
        : this(statusCode, "")
    {
    }

    public AppException(string message)
        : this(OperationStatusCode.ServerError, message)
    {
    }

    public AppException(OperationStatusCode statusCode, string message)
        : this(statusCode, message, HttpStatusCode.InternalServerError)
    {
    }

    public AppException(string message, object additionalData)
        : this(OperationStatusCode.ServerError, message, additionalData)
    {
    }

    public AppException(OperationStatusCode statusCode, object additionalData)
        : this(statusCode, "", additionalData)
    {
    }

    public AppException(OperationStatusCode statusCode, string message, object additionalData)
        : this(statusCode, message, HttpStatusCode.InternalServerError, additionalData)
    {
    }

    public AppException(OperationStatusCode statusCode, string message, HttpStatusCode httpStatusCode)
        : this(statusCode, message, httpStatusCode, null)
    {
    }

    public AppException(OperationStatusCode statusCode, string message, HttpStatusCode httpStatusCode, object additionalData)
        : this(statusCode, message, httpStatusCode, null, additionalData)
    {
    }

    public AppException(string message, Exception exception)
        : this(OperationStatusCode.ServerError, message, exception)
    {
    }

    public AppException(string message, Exception exception, object additionalData)
        : this(OperationStatusCode.ServerError, message, exception, additionalData)
    {
    }

    public AppException(OperationStatusCode statusCode, string message, Exception exception)
        : this(statusCode, message, HttpStatusCode.InternalServerError, exception)
    {
    }

    public AppException(OperationStatusCode statusCode, string message, Exception exception, object additionalData)
        : this(statusCode, message, HttpStatusCode.InternalServerError, exception, additionalData)
    {
    }

    public AppException(OperationStatusCode statusCode, string message, HttpStatusCode httpStatusCode, Exception exception)
        : this(statusCode, message, httpStatusCode, exception, "")
    {
    }

    public AppException(OperationStatusCode statusCode, string message, HttpStatusCode httpStatusCode, Exception exception, object additionalData)
        : base(message, exception)
    {
        ApiStatusCode = statusCode;
        HttpStatusCode = httpStatusCode;
        AdditionalData = additionalData;
    }
}