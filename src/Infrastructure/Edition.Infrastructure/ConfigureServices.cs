using StackExchange.Redis;
using Edition.Domain.Configuration;
using Microsoft.Extensions.Configuration;
using Edition.Domain.Caching.Abstractions;
using Edition.Infrastructure.SmsProviders;
using Edition.Application.Common.Contracts;
using Edition.Infrastructure.EmailProviders;
using Edition.Infrastructure.CacheProviders;
using Microsoft.Extensions.DependencyInjection;
using Edition.Infrastructure.CacheProviders.Redis;

namespace Edition.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        var redisConfiguration = configuration.GetSection("RedisConfiguration").Get<RedisConfiguration>();
        var redisHost = (redisConfiguration?.Hosts.FirstOrDefault())
                ?? throw new NullReferenceException($"{nameof(RedisConfiguration.Hosts)} is null in redis configuration!");

        var multiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
        {
            AllowAdmin = redisConfiguration.AllowAdmin,
            Ssl = redisConfiguration.Ssl,
            Password = redisConfiguration.Password,
            ConnectTimeout = redisConfiguration.ConnectTimeout,
            ConnectRetry = redisConfiguration.ConnectRetry,
            DefaultDatabase = redisConfiguration.Database,
            EndPoints = { $"{redisHost.Host}:{redisHost.Port}" }
        });

        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IEmailService, EmailServie>();

        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        services.AddSingleton<IDatabaseSelector, DatabaseSelector>();
        services.AddScoped<IRedisCache, RedisCache>();
        services.AddScoped<ICache, DistributedCache>();
        return services;
    }
}