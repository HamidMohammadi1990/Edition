using Edition.Application.Models;

namespace Edition.Application.Common.Contracts;

public interface ISeedService
{
    Task SeedDataAsync(List<DynamicPermission> permissions);
}