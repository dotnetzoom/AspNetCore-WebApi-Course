using System;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.User
{
    public class UserTokenHandler : BaseEntity
    {
        public int UserId { get; set; }

        public string AccessTokenHash { get; set; }

        public DateTimeOffset AccessTokenExpiresDateTime { get; set; }

        public string RefreshTokenIdHash { get; set; }

        public string RefreshTokenIdHashSource { get; set; }

        public DateTimeOffset RefreshTokenExpiresDateTime { get; set; }


        public virtual User User { get; set; }
    }

    public class UserTokenConfiguration : IEntityTypeConfiguration<UserTokenHandler>
    {
        public void Configure(EntityTypeBuilder<UserTokenHandler> builder)
        {
            
        }
    }
}
