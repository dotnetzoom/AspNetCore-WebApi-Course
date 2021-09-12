using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property<int>(x => x.Id).HasColumnName(@"Id").IsRequired(true).UseIdentityColumn().ValueGeneratedOnAdd();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        }
    }
}
