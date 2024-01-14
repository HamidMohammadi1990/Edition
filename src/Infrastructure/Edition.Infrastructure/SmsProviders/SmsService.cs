using Edition.Application.Common.Contracts;

namespace Edition.Infrastructure.SmsProviders;

public class SmsService : ISmsService
{
    public bool Send(string phoneNumber, string message)
    {
        return true;
    }
}