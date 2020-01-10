using Entities.AuditableEntity;
using Entities.User;
using Microsoft.AspNetCore.Identity;

namespace Entities.User
{
    public class UserClaim : IdentityUserClaim<int>, IAuditableEntity
    {
        public virtual User User { get; set; }
    }
}