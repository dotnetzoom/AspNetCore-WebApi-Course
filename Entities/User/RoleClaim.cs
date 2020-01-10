using Entities.AuditableEntity;
using Microsoft.AspNetCore.Identity;

namespace Entities.User
{
    public class RoleClaim : IdentityRoleClaim<int>, IAuditableEntity
    {
        public virtual Role Role { get; set; }
    }
}