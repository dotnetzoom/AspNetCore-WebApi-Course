using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Data.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property<Guid>(x => x.Id).HasColumnName(@"Id").IsRequired(true).UseIdentityColumn().ValueGeneratedOnAdd();
            builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).IsRequired();
            builder.HasOne(p => p.Category).WithMany(c => c.Posts).HasForeignKey(p => p.CategoryId);
            builder.HasOne(p => p.Author).WithMany(c => c.Posts).HasForeignKey(p => p.AuthorId);
        }
    }
}
