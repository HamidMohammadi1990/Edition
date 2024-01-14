using Edition.Domain.Attributes;
using Edition.Domain.Caching.Enums;
using Edition.Application.Common.Contracts;

namespace Edition.Application.Features.Users.Queries.GetUserInfo;

public class GetUserInfoQueryHandler
    (IEditionContext context)
    : IRequestHandler<GetUserInfoRequestQuery, GetUserInfoResponseDto?>
{
    [Cache(5, CacheInstanceType.Default)]
    public async Task<GetUserInfoResponseDto?> Handle(GetUserInfoRequestQuery request, CancellationToken cancellationToken = default)
    {
        var user = await context.User.FindAsync(request.Id, cancellationToken);
        if (user is null)
            return default;
        return new GetUserInfoResponseDto
            (user.Id, user.UserName, user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.Gender);
    }
}