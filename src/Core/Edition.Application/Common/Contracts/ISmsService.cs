namespace Edition.Application.Common.Contracts;

public interface ISmsService
{
    bool Send(string phoneNumber, string message);
}