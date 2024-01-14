using System.Text.Json.Serialization;
using Edition.Application.Common.Utilities.Security.Attributes;

namespace Edition.Application.Features.Users.Queries.GetUserInfo;

public class GetUserInfoRequestQuery : IRequest<GetUserInfoResponseDto?>
{
    [JsonConverter(typeof(UserEncryptor))]
    public int Id { get; set; }
}