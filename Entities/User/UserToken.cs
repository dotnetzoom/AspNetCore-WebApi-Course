using Entities.AuditableEntity;
using Microsoft.AspNetCore.Identity;

namespace Entities.User
{
    public class UserToken : IdentityUserToken<int>, IAuditableEntity
    {
        public virtual User User { get; set; }
    }
}