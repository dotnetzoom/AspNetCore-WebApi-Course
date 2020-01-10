using System.Collections.Generic;
using Entities.User;

namespace Entities.Identity
{
    public enum UserCardItemActiveTab
    {
        UserInfo,
        UserAdmin
    }

    public class UserCardItemViewModel
    {
        public User.User User { set; get; }
        public bool ShowAdminParts { set; get; }
        public List<Role> Roles { get; set; }
        public UserCardItemActiveTab ActiveTab { get; set; }
    }
}