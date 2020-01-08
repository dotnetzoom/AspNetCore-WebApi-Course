using System;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Post
{
    public class Post : BaseEntity
    {
        public string Title { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public DateTime Time { get; set; }
        public DateTime TimeToRead { get; set; }
        public string Image { get; set; }
        public int View { get; set; }
        public int Rank { get; set; }
        public int Type { get; set; }

        public int CategoryId { get; set; }
        public int UserId { get; set; }

        public Category Category { get; set; }
        public User.User User { get; set; }
    }

    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property(p => p.Title).IsRequired().HasMaxLength(300);
            builder.Property(p => p.Address).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Image).IsRequired().HasMaxLength(300);
            builder.Property(p => p.ShortDescription).IsRequired().HasMaxLength(800);
            builder.Property(p => p.Description).IsRequired();

            builder.HasOne(p => p.Category).WithMany(c => c.Posts).HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.User).WithMany(c => c.Posts).HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => a.UserId).HasName("IX_Post_UserId").IsUnique();
            builder.HasIndex(a => a.Type).HasName("IX_Post_Type").IsUnique();
            builder.HasIndex(a => a.Rank).HasName("IX_Post_Rank").IsUnique();
            builder.HasIndex(a => a.View).HasName("IX_Post_View").IsUnique();
            builder.HasIndex(a => a.Address).HasName("IX_Post_Address").IsUnique();
        }
    }
}
