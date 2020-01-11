using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Identity.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User.User>
    {
        public void Configure(EntityTypeBuilder<User.User> builder)
        {
            builder.ToTable("AppUsers");
        }
    }
}