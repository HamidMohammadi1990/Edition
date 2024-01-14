using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Edition.Application.Common.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Edition.Infrastructure.Persistence.SeedData;

namespace Edition.Infrastructure.Persistence;

public static class ConfigureServices
{
    public static IServiceCollection RegisterPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EditionDbContext>(options =>
             options.UseSqlServer(configuration.GetConnectionString("EditionDbContext")));
        
        services.AddScoped<IEditionContext, EditionDbContext>();
        services.AddScoped<ISeedService, SeedService>();
        return services;
    }
}