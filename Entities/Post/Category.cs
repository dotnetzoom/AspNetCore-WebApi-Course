using Entities.Common;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Post
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }

        public Category ParentCategory { get; set; }

        public ICollection<Category> ChildCategories { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);

            builder.HasOne(p => p.ParentCategory).WithMany(c => c.ChildCategories).HasForeignKey(p => p.ParentCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => a.ParentCategoryId).HasName("IX_Category_ParentCategoryId").IsUnique();
        }
    }
}
