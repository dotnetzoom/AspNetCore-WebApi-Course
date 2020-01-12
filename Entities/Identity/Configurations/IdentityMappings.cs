using Entities.Identity.Settings;
using Microsoft.EntityFrameworkCore;

namespace Entities.Identity.Configurations
{
    public static class IdentityMappings
    {
        public static void AddCustomIdentityMappings(this ModelBuilder modelBuilder, IdentitySiteSettings siteSettings)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityMappings).Assembly);

            // IEntityTypeConfiguration's which have constructors with parameters
            modelBuilder.ApplyConfiguration(new AppSqlCacheConfiguration(siteSettings));
        }
    }
}