using Edition.Application.Common.Contracts;

namespace Edition.Infrastructure.EmailProviders;

public class EmailServie : IEmailService
{
    public bool Send()
    {
        return true;
    }
}