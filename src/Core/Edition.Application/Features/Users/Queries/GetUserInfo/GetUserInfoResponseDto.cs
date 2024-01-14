using Edition.Domain.Enums;
using System.Text.Json.Serialization;
using Edition.Application.Common.Utilities.Security.Attributes;

namespace Edition.Application.Features.Users.Queries.GetUserInfo;

public class GetUserInfoResponseDto(int id, string userName, string? firstName, string? lastName, string? email, string phoneNumber, Gender? gende)
{
    [JsonConverter(typeof(UserEncryptor))]
    public int Id { get; set; } = id;
    public string UserName { get; set; } = userName;
    public string? FirstName { get; set; } = firstName;
    public string? LastName { get; set; } = lastName;
    public string? Email { get; set; } = email;
    public string PhoneNumber { get; set; } = phoneNumber;
    public Gender? Gende { get; set; } = gende;
}