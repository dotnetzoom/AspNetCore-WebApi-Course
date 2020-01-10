using System;
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
using Swashbuckle.AspNetCore.Filters;

namespace WebFramework.Swagger
{
    public static class SwaggerConfigurationExtensions
    {
        [Obsolete]
        public static void AddSwagger(this IServiceCollection services)
        {
            Assert.NotNull(services, nameof(services));

            #region AddSwaggerExamples
            //Add services to use Example Filters in swagger
            //If you want to use the Request and Response example filters (and have called options.ExampleFilters() above), then you MUST also call
            //This method to register all ExamplesProvider classes form the assembly
            //services.AddSwaggerExamplesFromAssemblyOf<PersonRequestExample>();

            //We call this method for by reflection with the Startup type of entry assembly (MyApi assembly)
            var mainAssembly = Assembly.GetEntryAssembly(); // => MyApi project assembly
            if (mainAssembly != null)
            {
                var mainType = mainAssembly.GetExportedTypes()[0];

                const string methodName = nameof(Swashbuckle.AspNetCore.Filters.ServiceCollectionExtensions.AddSwaggerExamplesFromAssemblyOf);
                var method = typeof(Swashbuckle.AspNetCore.Filters.ServiceCollectionExtensions).GetMethod(methodName);
                if (method != null)
                {
                    MethodInfo generic = method.MakeGenericMethod(mainType);
                    generic.Invoke(null, new[] { services });
                }
            }

            #endregion

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

                options.ExampleFilters();

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

                //OAuth2Scheme
                options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                {
                    //Scheme = "Bearer",
                    //In = ParameterLocation.Header,
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri("https://localhost:5001/api/v1/users/Token"),
                            //AuthorizationUrl = new Uri("https://localhost:5001/api/v1/users/Token")
                            //Scopes = new Dictionary<string, string>
                            //{
                            //    { "readAccess", "Access read operations" },
                            //    { "writeAccess", "Access write operations" }
                            //}
                        }
                    }
                });
                #endregion

                #region Add UnAuthorized to Response
                //Add 401 response and security requirements (Lock icon) to actions that need authorization
                options.OperationFilter<UnauthorizedResponsesOperationFilter>(true, "OAuth2");
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

            //ReDoc UI middleware. ReDoc UI is an alternative to swagger-ui
            app.UseReDoc(options =>
            {
                options.SpecUrl("/swagger/v1/swagger.json");
                //options.SpecUrl("/swagger/v2/swagger.json");

                #region Customizing
                //By default, the ReDoc UI will be exposed at "/api-docs"
                //options.RoutePrefix = "docs";
                //options.DocumentTitle = "My API Docs";

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
                #endregion
            });
        }
    }
}
