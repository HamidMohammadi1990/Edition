using Edition.Application.Enums;

namespace Edition.Application.Features.Users.Queries.UserNameCheck;

public record UserNameCheckResponseDto
(
    string? Phone,
    bool? HasAccount,
    LoginMethod? LoginMethod,
    int? SmsTTL,
    bool? HasPassword
);