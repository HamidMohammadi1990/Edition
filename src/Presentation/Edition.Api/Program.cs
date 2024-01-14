using Edition.Api.Modules;
using Edition.Application;
using Edition.Common.Models;
using Edition.Infrastructure;
using Edition.Api.Extensions;
using Edition.WebFramework.Swagger;
using Edition.WebFramework.Middlewares;
using Edition.Infrastructure.Persistence;
using Edition.WebFramework.Configuration;
using Edition.Application.Common.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCustomCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterApplicationServices()
                .RegisterInfrastructureServices(builder.Configuration)
                .RegisterPersistenceServices(builder.Configuration);

var siteSettingsConfiguration = builder.Configuration.GetSection(nameof(SiteSettings));
var siteSettings = siteSettingsConfiguration.Get<SiteSettings>();

builder.Services.AddSwagger();
builder.Services.Configure<SiteSettings>(siteSettingsConfiguration);
builder.Services.AddMinimalMvc();
builder.Services.AddElmahCore(builder.Configuration, siteSettings.ElmahPath);
builder.Services.AddJwtAuthentication(siteSettings.JwtSettings);
builder.Services.AddCustomApiVersioning();
builder.Services.AddCustomResponseCompression();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();
    var permissions = PermissionModule.GetPermissions();
    seedService.SeedDataAsync(permissions).GetAwaiter().GetResult();
}

if (!builder.Environment.IsDevelopment())
    app.UseHsts();

app.UseCustomExceptionHandler();
app.UseCors("CustomCors");
app.UseHttpsRedirection();
app.UseElmahCore(siteSettings);
app.UseSwaggerAndUI();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();