using FluentValidation;
using System.Reflection;
using Edition.Application.Services;
using Edition.Application.Common.Behaviors;
using Edition.Application.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Edition.Application;

public static class ConfigureServices
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));            
        });
        services.AddScoped<IAccountingService, AccountingService>();
        return services;
    }
}