namespace Edition.Common.Exceptions;

public class AppValidationException : Exception
{
    public AppValidationException()
       : base("One or more validation failures have occurred.") { }

    public AppValidationException(List<string> errors)
        => Errors = errors;

    public AppValidationException(string error)
        => Errors = [error];


    public List<string> Errors { get; } = [];
}