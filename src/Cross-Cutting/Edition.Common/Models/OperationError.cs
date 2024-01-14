namespace Edition.Common.Models;

public class OperationError(string message)
{
    public string Message { get; set; } = message;
}