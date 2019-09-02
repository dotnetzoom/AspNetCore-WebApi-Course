using System;
using Common;
using WebFramework.Swagger;
using WebFramework.Middlewares;
using WebFramework.Configuration;
using WebFramework.CustomMapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyApi
{
    public class Startup
    {
        private readonly SiteSettings _siteSetting;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            AutoMapperConfiguration.InitializeAutoMapper();

            _siteSetting = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }

        [Obsolete]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));

            services.AddDbContext(Configuration);

            services.AddCustomIdentity(_siteSetting.IdentitySettings);

            services.AddMinimalMvc();

            //services.AddElmah(Configuration, _siteSetting);

            services.AddJwtAuthentication(_siteSetting.JwtSettings);

            services.AddCustomApiVersioning();

            services.AddSwagger();

            return services.BuildAutofacServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.IntializeDatabase();

            app.UseCustomExceptionHandler();

            app.UseHsts(env);

            //app.UseElmah();

            app.UseHttpsRedirection();

            app.UseSwaggerAndUI();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}