using System;

namespace Entities
{
    public class Post : BaseEntity<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }

        public Category Category { get; set; }
        public User Author { get; set; }
    }

    //Configuration settings of poco classes should be kept in the Configurations folder in the Data project.

    //public class PostConfiguration : IEntityTypeConfiguration<Post>
    //{
    //    public void Configure(EntityTypeBuilder<Post> builder)
    //    {
    //        builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
    //        builder.Property(p => p.Description).IsRequired();
    //        builder.HasOne(p => p.Category).WithMany(c => c.Posts).HasForeignKey(p => p.CategoryId);
    //        builder.HasOne(p => p.Author).WithMany(c => c.Posts).HasForeignKey(p => p.AuthorId);
    //    }
    //}
}
