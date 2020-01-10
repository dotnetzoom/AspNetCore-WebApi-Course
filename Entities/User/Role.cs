using System.Collections.Generic;
using Entities.AuditableEntity;
using Entities.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.User
{
    public class Role : IdentityRole<int>, IEntity, IAuditableEntity
    {
        public Role()
        {
        }

        public Role(string name)
            : this()
        {
            Name = name;
        }

        public Role(string name, string description)
            : this(name)
        {
            Description = description;
        }

        public string Description { get; set; }

        public virtual ICollection<UserRole> Users { get; set; }

        public virtual ICollection<RoleClaim> Claims { get; set; }
    }

    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Description).HasMaxLength(100);

            builder.HasIndex(a => a.Name).HasName("IX_Role_Name").IsUnique();
        }
    }
}
