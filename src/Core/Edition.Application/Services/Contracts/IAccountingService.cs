using Edition.Common.Models;
using Edition.Domain.Entities;
using Edition.Application.Models.Services;

namespace Edition.Application.Services.Contracts;

public interface IAccountingService
{
    Task<OperationResult<LogOutTokenResponseDto>> BlockTokenAsync(CheckTokenRequestDto request);
    OperationResult<bool> IsTokenBlocked(CheckTokenRequestDto request);
    Task<OperationResult<AccessTokenResponse>> GenerateTokenAsync(User user);
    Task<OperationResult<AccessTokenResponse>> RefreshTokenAsync(RefreshTokenRequestDto request);
}