using System;
using Entities.Identity.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WebFramework.Configuration.Identity
{
    public static class IdentityServicesRegistry
    {
        /// <summary>
        /// Adds all of the ASP.NET Core Identity related services and configurations at once.
        /// </summary>
        public static void AddCustomIdentityServices(this IServiceCollection services,IdentitySiteSettings identitySiteSettings)
        {
            //var siteSettings = GetSiteSettings(services);
            services.AddIdentityOptions(identitySiteSettings);
            services.AddConfiguredDbContext(identitySiteSettings);
            services.AddCustomServices();
            services.AddCustomTicketStore(identitySiteSettings);
            services.AddDynamicPermissions();
            services.AddCustomDataProtection(identitySiteSettings);
        }

        public static IdentitySiteSettings GetSiteSettings(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var siteSettingsOptions = provider.GetRequiredService<IOptionsSnapshot<IdentitySiteSettings>>();
            var siteSettings = siteSettingsOptions.Value;
            if (siteSettings == null) throw new ArgumentNullException(nameof(siteSettings));
            return siteSettings;
        }
    }
}