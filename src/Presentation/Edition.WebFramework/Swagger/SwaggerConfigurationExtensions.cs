using ElmahCore.Mvc;
using Asp.Versioning;
using System.Reflection;
using Edition.Common.Models;
using Edition.Common.Utilities;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Edition.WebFramework.Swagger;

public static class SwaggerConfigurationExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        Assert.NotNull(services, nameof(services));

        services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

        //Add services and configuration to use swagger
        services.AddSwaggerGen(options =>
        {
            var xmlDocPath = Path.Combine(AppContext.BaseDirectory, "Edition.Api.xml");
            //show controller XML comments like summary
            options.IncludeXmlComments(xmlDocPath, true);
            options.EnableAnnotations();
            options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "API V1" });
            options.SwaggerDoc("v2", new OpenApiInfo { Version = "v2", Title = "API V2" });

            //Enable to use [SwaggerRequestExample] & [SwaggerResponseExample]
            options.ExampleFilters();

            //Set summary of action if not already set
            options.OperationFilter<ApplySummariesOperationFilter>();

            #region Add UnAuthorized to Response
            //Add 401 response and security requirements (Lock icon) to actions that need authorization
            options.OperationFilter<UnauthorizedResponsesOperationFilter>(true, "OAuth2");
            #endregion

            #region Add Jwt Authentication            
            //OAuth2Scheme
            options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri("/api/v1/account/sign-in", UriKind.Relative)
                    }
                }
            });
            #endregion

            //Versioning
            // Remove version parameter from all Operations
            options.OperationFilter<RemoveVersionParameters>();

            //set version "api/v{version}/[controller]" from current swagger doc verion
            options.DocumentFilter<SetVersionInPaths>();

            //Seperate and categorize end-points by doc version
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                var versions = methodInfo.DeclaringType
                    .GetCustomAttributes<ApiVersionAttribute>(true)
                    .SelectMany(attr => attr.Versions);

                return versions.Any(v => $"v{v}" == docName);
            });
        });
    }

    public static IApplicationBuilder UseElmahCore(this IApplicationBuilder app, SiteSettings siteSettings)
    {
        Assert.NotNull(app, nameof(app));
        Assert.NotNull(siteSettings, nameof(siteSettings));

        app.UseWhen(context => context.Request.Path.StartsWithSegments(siteSettings.ElmahPath, StringComparison.OrdinalIgnoreCase), appBuilder =>
        {
            appBuilder.Use((ctx, next) =>
            {
                ctx.Features.Get<IHttpBodyControlFeature>().AllowSynchronousIO = true;
                return next();
            });
        });
        app.UseElmah();

        return app;
    }

    public static IApplicationBuilder UseSwaggerAndUI(this IApplicationBuilder app)
    {
        Assert.NotNull(app, nameof(app));

        //Swagger middleware for generate "Open API Documentation" in swagger.json
        app.UseSwagger();

        //Swagger middleware for generate UI from swagger.json
        app.UseSwaggerUI(options =>
        {
            options.DisplayRequestDuration();
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "V2 Docs");
            options.DocExpansion(DocExpansion.None);
        });

        //ReDoc UI middleware. ReDoc UI is an alternative to swagger-ui
        app.UseReDoc(options =>
        {
            options.SpecUrl("/swagger/v1/swagger.json");

            options.EnableUntrustedSpec();
            options.ScrollYOffset(10);
            options.HideHostname();
            options.HideDownloadButton();
            options.ExpandResponses("200,201");
            options.RequiredPropsFirst();
            options.NoAutoAuth();
            options.PathInMiddlePanel();
            options.HideLoading();
            options.NativeScrollbars();
            options.DisableSearch();
            options.OnlyRequiredInSamples();
            options.SortPropsAlphabetically();
        });
        return app;
    }
}