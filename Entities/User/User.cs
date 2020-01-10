using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.AuditableEntity;
using Entities.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.User
{
    public class User : IdentityUser<int>, IEntity, IAuditableEntity
    {
        public User()
        {
            IsActive = true;
        }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [NotMapped]
        public string DisplayName
        {
            get
            {
                var displayName = $"{FirstName} {LastName}";
                return string.IsNullOrWhiteSpace(displayName) ? UserName : displayName;
            }
        }

        [StringLength(450)]
        public string PhotoFileName { get; set; }
        public DateTime? Birthday { get; set; }
        public GenderType Gender { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public DateTime? LastVisitDateTime { get; set; }
        public bool IsEmailPublic { get; set; }
        public string Location { set; get; }

        public ICollection<Post.Post> Posts { get; set; }

        public virtual ICollection<UserUsedPassword> UserUsedPasswords { get; set; }

        public virtual ICollection<UserToken> UserTokens { get; set; }

        public virtual ICollection<UserRole> Roles { get; set; }

        public virtual ICollection<UserLogin> Logins { get; set; }

        public virtual ICollection<UserClaim> Claims { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(p => p.UserName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Email).IsRequired().HasMaxLength(100);
            builder.Property(p => p.PasswordHash).IsRequired().HasMaxLength(100);

            builder.HasIndex(a => a.Email).HasName("IX_User_Email").IsUnique();
            builder.HasIndex(a => a.PhoneNumber).HasName("IX_User_Phone").IsUnique();
        }
    }

    public enum GenderType
    {
        [Display(Name = "مرد")]
        Male = 1,

        [Display(Name = "زن")]
        Female = 2
    }
}
