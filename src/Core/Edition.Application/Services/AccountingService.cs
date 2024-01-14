using System.Text;
using Edition.Common.Models;
using System.Security.Claims;
using Edition.Domain.Entities;
using Microsoft.Extensions.Options;
using Edition.Domain.Caching.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Edition.Domain.Caching.Abstractions;
using Edition.Application.Models.Services;
using Edition.Application.Common.Contracts;
using Edition.Application.Services.Contracts;

namespace Edition.Application.Services;

public class AccountingService
    (ICache cache, IEditionContext context,
    IOptions<TokenValidationParameters> tokenValidationParameters, IOptions<SiteSettings> siteSettings)
    : IAccountingService
{
    public async Task<OperationResult<LogOutTokenResponseDto>> BlockTokenAsync(CheckTokenRequestDto request)
    {
        var principal = GetPrincipalFromTokenWithoutAlgorithmValidation(request.Token);
        if (principal is null)
            return OperationResult<LogOutTokenResponseDto>.Success(new LogOutTokenResponseDto(true));

        var expiredDateString = principal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value;
        var expiredDuration = long.Parse(expiredDateString);
        var expiredDate = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(expiredDuration).ToLocalTime();
        if (expiredDate < DateTime.Now)
            return OperationResult<LogOutTokenResponseDto>.Success(new LogOutTokenResponseDto(true));

        var userId = principal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
        var jwtId = principal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        var cacheKey = GetBlockedTokenCacheKey(userId, jwtId);
        var cachedToken = cache.Get<string>(cacheKey, CacheInstanceType.UserTokens);
        if (string.IsNullOrEmpty(cachedToken))
            cache.Set(cacheKey, request.Token, expiredDate, CacheInstanceType.UserTokens);

        var storedToken = await GetRefreshTokenByTokenAsync(jwtId);
        if (storedToken == null)
            return OperationResult<LogOutTokenResponseDto>.Success(new LogOutTokenResponseDto(true));

        storedToken.Invalidated = true;
        await context.SaveAllChangesAsync();
        return OperationResult<LogOutTokenResponseDto>.Success(new LogOutTokenResponseDto(true));
    }

    public OperationResult<bool> IsTokenBlocked(CheckTokenRequestDto request)
    {
        var principal = GetPrincipalFromTokenWithoutAlgorithmValidation(request.Token);
        if (principal is null)
            return OperationResult<bool>.Success(true);

        var userId = principal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
        var jwtId = principal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        var cacheKey = GetBlockedTokenCacheKey(userId, jwtId);
        var tokenCache = cache.Get<string>(cacheKey, CacheInstanceType.UserTokens);
        if (string.IsNullOrEmpty(tokenCache))
            return OperationResult<bool>.Fail(false);

        return OperationResult<bool>.Success(true);
    }

    public async Task<OperationResult<AccessTokenResponse>> GenerateTokenAsync(User user)
    {
        var secretKey = Encoding.UTF8.GetBytes(siteSettings.Value.JwtSettings.SecretKey); // longer that 16 character
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);
        var encryptionkey = Encoding.UTF8.GetBytes(siteSettings.Value.JwtSettings.EncryptKey); //must be 16 character
        var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

        var userRoles = await GetRolesByUserId(user.Id);
        var claims = GenerateClaims(user, userRoles);
        var refreshToken = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)!.Value;
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = siteSettings.Value.JwtSettings.Issuer,
            Audience = siteSettings.Value.JwtSettings.Audience,
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now.AddMinutes(siteSettings.Value.JwtSettings.NotBeforeMinutes),
            Expires = DateTime.Now.AddMinutes(siteSettings.Value.JwtSettings.ExpirationMinutes),
            SigningCredentials = signingCredentials,
            EncryptingCredentials = encryptingCredentials,
            Subject = new ClaimsIdentity(claims)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);
        return OperationResult<AccessTokenResponse>.Success(new AccessTokenResponse(securityToken, refreshToken));
    }

    public async Task<OperationResult<AccessTokenResponse>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var validateToken = GetPrincipalFromToken(request.Token);
        if (validateToken is null)
            return OperationResult<AccessTokenResponse>.Fail("Invalid Token");
        var expiredDate = validateToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value;

        var expiredDateValue = long.Parse(expiredDate);
        var expiredDateUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiredDateValue);

        if (expiredDateUtc > DateTime.UtcNow)
            return OperationResult<AccessTokenResponse>.Fail("This token hasn't expired yet");

        var storedToken = await GetRefreshTokenByTokenAsync(request.RefreshToken);
        if (storedToken is null)
            OperationResult<AccessTokenResponse>.Fail("Invalid refresh token");

        if (storedToken!.ExpiredDate < DateTime.UtcNow)
            return OperationResult<AccessTokenResponse>.Fail("This refresh token has expired");

        var jti = validateToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        if (storedToken.JwtId != jti)
            return OperationResult<AccessTokenResponse>.Fail("This refresh token dose not match JWT");

        storedToken.Used = true;
        await context.SaveAllChangesAsync();
        var userId = Convert.ToInt32(validateToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Sid).Value);
        var user = await FindUserByIdAsync(userId);
        return await GenerateTokenAsync(user);
    }

    private async Task<List<Role>> GetRolesByUserId(int userId)
    {
        var roles =
            await (from UserRole in context.UserRole
                   join Role in context.Role
                   on UserRole.RoleId equals Role.Id
                   where UserRole.UserId == userId
                   select Role)
                   .AsNoTracking()
                   .ToListAsync();
        return roles;
    }

    private static List<Claim> GenerateClaims(User user, List<Role> roles)
    {
        var jwtId = Guid.NewGuid().ToString();
        var securityStampClaimType = new ClaimsIdentityOptions().SecurityStampClaimType;
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, jwtId),
            new(securityStampClaimType, user.SecurityStamp)
        };
        claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x.Title)));
        return claims;
    }

    private static bool IsJwtValidSecurityAlgorithm(SecurityToken securityToken)
    {
        return (securityToken is JwtSecurityToken jwtSecurityToken) &&
               jwtSecurityToken
               .Header.Alg
               .Equals(SecurityAlgorithms.HmacSha256,
               StringComparison.InvariantCultureIgnoreCase);
    }

    private ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenValidationParameters.Value.ValidateLifetime = false;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters.Value, out var validationToken);
            if (!IsJwtValidSecurityAlgorithm(validationToken))
                return null;
            return principal;
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            tokenValidationParameters.Value.ValidateLifetime = true;
        }
    }

    private ClaimsPrincipal GetPrincipalFromTokenWithoutAlgorithmValidation(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenValidationParameters.Value.ValidateLifetime = false;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters.Value, out var validationToken);
            return principal;
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            tokenValidationParameters.Value.ValidateLifetime = true;
        }
    }

    private static string GetBlockedTokenCacheKey(string userId, string jwtId)
        => $"BlockedToken|{userId}|{jwtId}";

    private async Task<User> FindUserByIdAsync(int userId)
        => await context.User.SingleAsync(x => x.Id == userId);

    private async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token)
        => await context.RefreshToken.SingleOrDefaultAsync(x => x.JwtId == token && !x.Invalidated && !x.Used);
}