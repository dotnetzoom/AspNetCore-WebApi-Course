using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Utilities;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace WebFramework.Swagger
{
    public static class SwaggerConfigurationExtensions
    {
        [Obsolete]
        public static void AddSwagger(this IServiceCollection services)
        {
            Assert.NotNull(services, nameof(services));

            //Add services to use Example Filters in swagger
            //services.AddSwaggerExamples();
            //services.AddSwaggerExamplesFromAssemblyOf<MyExample>();

            //Add services and configuration to use swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API",
                    Description = "ASP.NET Core Web API",
                    TermsOfService = new Uri("https://mhkarami97.github.io"),
                    Contact = new OpenApiContact
                    {
                        Name = "Mohammad Hossein Karami",
                        Email = "mhkarami1997@gmail.com",
                        Url = new Uri("https://mhkarami97.github.io"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under GPL",
                        Url = new Uri("https://mhkarami97.github.io"),
                    }
                });
                options.SwaggerDoc("v2", new OpenApiInfo { Version = "v2", Title = "API V2" });

                var xmlDocPath = Path.Combine(AppContext.BaseDirectory, "MyApi.xml");
                //show controller XML comments like summary
                options.IncludeXmlComments(xmlDocPath, true);

                options.EnableAnnotations();
                options.DescribeAllEnumsAsStrings();
                //options.DescribeAllParametersInCamelCase();
                //options.DescribeStringEnumsInCamelCase()
                //options.UseReferencedDefinitionsForEnums()
                //options.IgnoreObsoleteActions();
                //options.IgnoreObsoleteProperties();

                //options.ExampleFilters();

                #region Filters
                //Enable to use [SwaggerRequestExample] & [SwaggerResponseExample]
                //options.ExampleFilters();

                //Adds an Upload button to endpoints which have [AddSwaggerFileUploadButton]
                //options.OperationFilter<AddFileParamTypesOperationFilter>();

                //Set summary of action if not already set
                options.OperationFilter<ApplySummariesOperationFilter>();

                #region Add Jwt Authentication
                //Add Lockout icon on top of swagger ui page to authenticate
                //options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                //{
                //    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                //    Name = "Authorization",
                //    In = "header"
                //});
                //options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                //{
                //    {"Bearer", new string[] { }}
                //});

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Scheme = "Bearer",
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345deface\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            Scopes = new Dictionary<string, string>
                            {
                                { "readAccess", "Access read operations" },
                                { "writeAccess", "Access write operations" }
                            },
                            TokenUrl = new Uri("https://localhost:44339/api/v1/users/Token")
                        }
                    }
                });
                #endregion

                #region Add UnAuthorized to Response
                //Add 401 response and security requirements (Lock icon) to actions that need authorization
                var securityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345deface\"",
                    Name = "Authorization",
                    Type = SecuritySchemeType.OAuth2,
                    In = ParameterLocation.Header
                };
                options.OperationFilter<UnauthorizedResponsesOperationFilter>(true, securityScheme);
                #endregion

                #region Versioning
                // Remove version parameter from all Operations
                options.OperationFilter<RemoveVersionParameters>();

                //set version "api/v{version}/[controller]" from current swagger doc version
                options.DocumentFilter<SetVersionInPaths>();

                //Separate and categorize end-points by doc version
                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return false;

                    var versions = (methodInfo.DeclaringType ?? throw new InvalidOperationException())
                        .GetCustomAttributes<ApiVersionAttribute>(true)
                        .SelectMany(attr => attr.Versions);

                    return versions.Any(v => $"v{v.ToString()}" == docName);
                });
                #endregion

                //If use FluentValidation then must be use this package to show validation in swagger (MicroElements.Swashbuckle.FluentValidation)
                //options.AddFluentValidationRules();
                #endregion
            });
        }

        public static void UseSwaggerAndUi(this IApplicationBuilder app)
        {
            Assert.NotNull(app, nameof(app));

            //Swagger middleware for generate "Open API Documentation" in swagger.json
            app.UseSwagger(options =>
            {
                //options.RouteTemplate = "api-docs/{documentName}/swagger.json";
            });

            //Swagger middleware for generate UI from swagger.json
            app.UseSwaggerUI(options =>
            {
                #region Customizing
                //// Display
                //options.DefaultModelExpandDepth(2);
                //options.DefaultModelRendering(ModelRendering.Model);
                //options.DefaultModelsExpandDepth(-1);
                //options.DisplayOperationId();
                //options.DisplayRequestDuration();
                options.DocExpansion(DocExpansion.None);
                options.EnableDeepLinking();
                //options.EnableFilter();
                //options.MaxDisplayedTags(5);
                options.ShowExtensions();

                //// Network
                options.EnableValidator();
                //options.SupportedSubmitMethods(SubmitMethod.Get);

                //// Other
                //options.DocumentTitle = "CustomUIConfig";
                options.InjectStylesheet("/style/custom-stylesheet.css");
                //options.InjectJavascript("/ext/custom-javascript.js");
                //options.RoutePrefix = "api-docs";
                #endregion

                options.OAuthClientId("test-id");
                options.OAuthClientSecret("test-secret");
                options.OAuthRealm("test-realm");
                options.OAuthAppName("test-app");
                options.OAuthScopeSeparator(" ");
                //options.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { { "foo", "bar" }}); 
                options.OAuthUseBasicAuthenticationWithAccessCodeGrant();

                options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "V2 Docs");
            });
        }
    }
}
