using Edition.Domain.Entities;
using Edition.Common.Extensions;
using Edition.Application.Enums;
using Edition.Common.Exceptions;
using Edition.Application.Common.Contracts;

namespace Edition.Application.Features.Users.Queries.UserNameCheck;

public class UserNameCheckQueryHandler
    (IEditionContext context, ISmsService smsService, IEmailService emailService)
    : IRequestHandler<UserNameCheckRequestQuery, UserNameCheckResponseDto>
{
    public async Task<UserNameCheckResponseDto> Handle(UserNameCheckRequestQuery request, CancellationToken cancellationToken)
    {
        User? user = user = await context.User
            .SingleOrDefaultAsync(x => x.UserName == request.UserName ||
                                  x.Email == request.UserName, cancellationToken);
        if (user is not null && request.UserName.IsMobile())
        {
            var succeeded = smsService.Send(request.UserName, "");
            if (!succeeded)
                throw new AppValidationException("لطفا دقایقی دیگر اقدام به ورود نمایید");
            return new UserNameCheckResponseDto(request.UserName, true, LoginMethod.OTP, 180, false);
        }

        if (user is null && request.UserName.IsMobile())
        {
            var succeeded = smsService.Send(request.UserName, "");
            if (!succeeded)
                throw new AppValidationException("لطفا دقایقی دیگر اقدام نمایید");
            return new UserNameCheckResponseDto(request.UserName, null, null, 180, null);
        }

        if (user is not null && request.UserName.IsEmail())
        {
            emailService.Send();
            return new UserNameCheckResponseDto(null, true, LoginMethod.Password, null, true);
        }

        throw new AppValidationException("حساب کاربری با مشخصات وارد شده وجود ندارد. لطفا از شماره تلفن همراه برای ساخت حساب کاربری استفاده نمایید");
    }
}