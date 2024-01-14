using System.Net;
using System.Text;
using Newtonsoft.Json;
using Edition.Api.Filters;
using Edition.Common.Enums;
using Edition.Common.Models;
using System.Security.Claims;
using Edition.Common.Extensions;
using Edition.Common.Exceptions;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Edition.Application.Models.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Edition.Application.Services.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Edition.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMinimalMvc(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add(new AuthorizeFilter()); //Apply AuthorizeFilter as global filter to all actions
            options.Filters.Add<PermissionAuthorizeAttribute>();
        }).AddNewtonsoftJson(option =>
        {
            option.SerializerSettings.Converters.Add(new StringEnumConverter());
            option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        });
        services.AddSwaggerGenNewtonsoftSupport();
    }

    public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
    {
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
        var encryptionKey = Encoding.UTF8.GetBytes(jwtSettings.EncryptKey);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero, // default: 5 min
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateAudience = true, //default : false
            ValidAudience = jwtSettings.Audience,
            ValidateIssuer = true, //default : false
            ValidIssuer = jwtSettings.Issuer,
            TokenDecryptionKey = new SymmetricSecurityKey(encryptionKey)
        };

        services.AddScoped(x => tokenValidationParameters);
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {          
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = tokenValidationParameters;
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception != null)
                        throw new AppException(OperationStatusCode.UnAuthorized, "Authentication failed.", HttpStatusCode.Unauthorized, context.Exception, null);

                    return Task.CompletedTask;
                },
                OnTokenValidated = async context =>
                {
                    var accountingService = context.HttpContext.RequestServices.GetRequiredService<IAccountingService>();

                    var claimsIdentity = context.Principal!.Identity as ClaimsIdentity;
                    if (claimsIdentity!.Claims?.Any() != true)
                        context.Fail("This token has no claims.");

                    var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                    if (!securityStamp.HasValue())
                        context.Fail("This token has no security stamp");

                    var accessToken = context.SecurityToken as JwtSecurityToken;
                    var isBlocked = accountingService.IsTokenBlocked(new CheckTokenRequestDto(accessToken?.RawData));
                    if (isBlocked.Result)
                        context.Fail("Invalid Token");
                },
                OnChallenge = context =>
                {
                    if (context.AuthenticateFailure != null)
                        throw new AppException(OperationStatusCode.UnAuthorized, "Authenticate failure.", HttpStatusCode.Unauthorized, context.AuthenticateFailure, null);
                    throw new AppException(OperationStatusCode.UnAuthorized, "You are unauthorized to access this resource.", HttpStatusCode.Unauthorized);
                }
            };
        });
    }
}