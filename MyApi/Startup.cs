using System;
using AspNetCoreRateLimit;
using Autofac;
using Common;
using Common.WebToolkit;
using Entities.Identity.Settings;
using WebFramework.Swagger;
using WebFramework.Middlewares;
using WebFramework.Configuration;
using WebFramework.CustomMapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OwaspHeaders.Core.Extensions;
using WebFramework.Configuration.Identity;

namespace MyApi
{
    public class Startup
    {
        private readonly SiteSettings _siteSetting;
        private readonly IdentitySiteSettings _identitySiteSettings;

        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            _siteSetting = Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
            _identitySiteSettings = Configuration.GetSection(nameof(IdentitySiteSettings)).Get<IdentitySiteSettings>();
        }

        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));

            services.Configure<IdentitySiteSettings>(Configuration.GetSection(nameof(IdentitySiteSettings)));

            services.InitializeAutoMapper();

            services.AddDbContext(Configuration);

            //services.AddCustomIdentity(_siteSetting.IdentitySettings);

            // Adds all of the ASP.NET Core Identity related services and configurations at once.
            services.AddCustomIdentityServices(_identitySiteSettings);

            services.AddMinimalMvc();

            services.AddJwtAuthentication(_siteSetting.JwtSettings);

            services.AddCustomApiVersioning();

            services.AddMemoryCache();

            services.AddSwagger();

            services.AddHttpCacheHeaders(
                (expirationModelOptions) =>
                {
                    expirationModelOptions.MaxAge = 600;
                },
                (validationModelOptions) =>
                {
                    validationModelOptions.MustRevalidate = true;
                });

            // configure ip rate limiting middle-ware
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            // configure client rate limiting middleware
            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            //services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            var opt = new ClientRateLimitOptions();
            Configuration.GetSection("ClientRateLimiting").Bind(opt);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            //services.AddElmah(Configuration, _siteSetting);
        }

        public void ConfigureContainer(ContainerBuilder builder) {
            
            builder.RegisterModule(new AutofacConfigurationExtensions());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseIpRateLimiting();

            app.UseClientRateLimiting();

            app.IntializeDatabase();

            app.UseCustomExceptionHandler();

            app.UseHsts();

            //app.UseElmah();

            app.UseContentSecurityPolicy();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseSwaggerAndUi();

            app.UseRouting();

            app.UseSecureHeadersMiddleware(SecureHeadersMiddlewareConfiguration.CustomConfiguration());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseHttpCacheHeaders();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}