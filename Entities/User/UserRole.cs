using Entities.AuditableEntity;
using Microsoft.AspNetCore.Identity;

namespace Entities.User
{
    public class UserRole : IdentityUserRole<int>, IAuditableEntity
    {
        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
}