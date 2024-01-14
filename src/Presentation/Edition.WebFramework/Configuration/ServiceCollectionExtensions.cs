using ElmahCore.Sql;
using ElmahCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.ResponseCompression;

namespace Edition.WebFramework.Configuration;

public static class ServiceCollectionExtensions
{
    public static void AddElmahCore(this IServiceCollection services, IConfiguration configuration, string elmahPath)
    {
        services.AddElmah<SqlErrorLog>(options =>
        {
            options.Path = elmahPath;
            options.ConnectionString = configuration.GetConnectionString("Elmah");
        });
    }    
    public static void AddCustomApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });
    }
    public static void AddCustomResponseCompression(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
        });
    }
    public static void AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy("CustomCors", builder =>
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader()
        ));
    }
}